namespace ScannerClient.WebApp.Core.Scanner.Models
{
    public class ScannedSymbolData
    {
        public DateTime TimeStamp { get; set; }
        public string Key { get; set; }

        public ScannedSymbolData(DateTime timeStamp, string key)
        {
            TimeStamp = timeStamp;
            Key = key;
        }
    }
}
