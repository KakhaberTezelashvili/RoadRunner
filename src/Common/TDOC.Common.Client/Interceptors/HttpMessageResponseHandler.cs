using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using TDOC.Common.Data.Model.Errors;

namespace TDOC.Common.Client.Interceptors;

public class HttpMessageResponseHandler : DelegatingHandler
{
    private readonly IExceptionService _exceptionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpMessageResponseHandler" /> class.
    /// </summary>
    /// <param name="exceptionService">Service provides methods to handle exceptions.</param>
    public HttpMessageResponseHandler(IExceptionService exceptionService)
    {
        _exceptionService = exceptionService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
            return response;

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Todo: redirect to login page
            //_navigationManager.NavigateTo("Login_Page");
            return response;
        }
        string errorsStr = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JObject.Parse(errorsStr);
        ValidationError[] errors = result["Errors"]?.ToObject<ValidationError[]>(new JsonSerializer
        {
            TypeNameHandling = TypeNameHandling.Objects,
            SerializationBinder = ValidationErrorKnownTypes.GetKnownTypes()
        });

        var badRequest =
            errors == null ? new HttpBadRequestException(new ValidationError { Message = "Bad request!" }) : new HttpBadRequestException(errors);
        await _exceptionService.ShowException(badRequest);

        // We can swallow "Bad request" response because it already handled in appropriate way.
        // Save us of catching the same exception in other places, e.g.: inside MediatorCarrier.
        return new HttpResponseMessage(HttpStatusCode.OK);
    }
}