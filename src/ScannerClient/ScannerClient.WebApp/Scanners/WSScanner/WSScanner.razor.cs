using Newtonsoft.Json;
using ProductionService.Client.Services.Barcodes;
using ProductionService.Shared.Dtos.Scanner;
using ScannerClient.WebApp.Scanners.WSScanner.Models;
using System.Net.WebSockets;
using System.Text;

namespace ScannerClient.WebApp.Scanners.WSScanner;

[Route($"/{ScannerUrls.WS}")]
public partial class WSScanner : IDisposable
{
    [Inject]
    private IBarcodeApiService _barcodeService { get; set; }

    private CancellationTokenSource disposalTokenSource;
    private ClientWebSocket webSocket;
    private readonly string valueVIDAndPID = "VID_0536&PID_020A";
    private readonly string logPath = "D:\\Temp\\SCS.log";
    private string log = "";
    private bool logTurnedOn;
    private readonly List<BarcodeDto> wsBarcodes = new();

    protected override void OnInitialized() => RefreshWebSocket();

    public void Dispose()
    {
        disposalTokenSource.Cancel();
        webSocket.Dispose();
    }

    public async Task ReconnectWebSocketAsync()
    {
        await DisconnectWebSocketAsync();
        await ConnectWebSocketAsync();
    }

    public async Task DisconnectWebSocketAsync()
    {
        try
        {
            disposalTokenSource.Cancel();
            if (IsConnected())
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Socket requested close", CancellationToken.None);
                log = "";
                logTurnedOn = false;
            }
        }
        catch (WebSocketException e)
        {
            // there is no connection to the Web Socket Server
            log += $"{e.Message} DisconnectAsync caught WebSocketException while disconnecting, ignoring\n";
            StateHasChanged();
        }
    }

    public bool IsConnected() => webSocket?.State == WebSocketState.Open;

    private void RefreshWebSocket()
    {
        ClientWebSocket oldSocket = webSocket;
        webSocket = new ClientWebSocket();
        oldSocket?.Dispose();
        disposalTokenSource = new CancellationTokenSource();
    }

    private async Task ConnectWebSocketAsync()
    {
        RefreshWebSocket();
        string uri = "ws://127.0.0.1:10000/";
        try
        {
            await webSocket.ConnectAsync(new Uri(uri), disposalTokenSource.Token);
            StateHasChanged();
            _ = ReceiveLoop();
        }
        catch (WebSocketException e)
        {
            // there is no connection to the Web Socket Server
            log += $"{e.Message} ConnectAsync caught WebSocketException while connecting to {uri}\n";
            StateHasChanged();
        }
    }

    private async void HelloWebSocketAsync()
    {
        string jsonData = $"{{\"type\":\"hello\",\"data\":\"{valueVIDAndPID}\"}}";
        await SendMessageAsync(jsonData);
    }

    private async void CloseWebSocketAsync()
    {
        string jsonData = "{\"type\":\"goodbye\"}";
        await SendMessageAsync(jsonData);
    }

    private async Task SendMessageAsync(string data)
    {
        if (data is null)
            data = "";
        if (!disposalTokenSource.IsCancellationRequested && IsConnected())
        {
            log += $"Tx: {data}\n";
            StateHasChanged();
            var dataToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(data));
            await webSocket.SendAsync(dataToSend, WebSocketMessageType.Text, true, disposalTokenSource.Token);
        }
    }

    private async Task ReceiveLoop()
    {
        var buffer = new ArraySegment<byte>(new byte[1024]);
        while (!disposalTokenSource.IsCancellationRequested)
        {
            // Note that the received block might only be part of a larger message. If this applies in your scenario,
            // check the received.EndOfMessage and consider buffering the blocks until that property is true.
            // Or use a higher-level library such as SignalR.
            WebSocketReceiveResult received = await webSocket.ReceiveAsync(buffer, disposalTokenSource.Token);
            string receivedAsText = Encoding.UTF8.GetString(buffer.Array, 0, received.Count);
            WSResponse wsResponse = JsonConvert.DeserializeObject<WSResponse>(receivedAsText);
            if (wsResponse.Type == "data")
            {
                BarcodeDto barcodeData = await _barcodeService.GetBarcodeDataAsync(wsResponse.Data);
                wsBarcodes.Add(barcodeData);
            }
            else
                log += $"Rx: {receivedAsText}\n";
            StateHasChanged();
        }
    }

    private async void LogOnAsync()
    {
        logTurnedOn = true;
        string jsonData = $"{{\"type\":\"logOn\",\"data\":\"{logPath.Replace("\\", "\\\\")}\"}}";
        await SendMessageAsync(jsonData);
    }

    private async void LogOffAsync()
    {
        logTurnedOn = false;
        string jsonData = "{\"type\":\"logOff\"}";
        await SendMessageAsync(jsonData);
    }
}