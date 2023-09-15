using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using TDOC.Common.Data.Model.Errors;
using TDOC.Common.Data.Models.Exceptions;
using TDOC.Common.Server.Validations;

namespace TDOC.Common.Server.Middlewares;

public class ApiExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;

    public ApiExceptionHandlingMiddleware(RequestDelegate next, ILogger<ApiExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        ProblemDetails problemDetails = ex switch
        {
            ValidationException => new CustomValidationProblemDetails(),
            _ => new ProblemDetails()
        };
        problemDetails.Instance = context.Request.Path;

        switch (ex)
        {
            case ValidationException validationException:
                {
                    ValidationError validationError = ex switch
                    {
                        DomainException => new DomainValidationError(),
                        InputArgumentException => new InputArgumentValidationError(),
                        _ => new ValidationError()
                    };

                    validationError.Code = validationException.Code;
                    validationError.Message = validationException.Message;
                    validationError.Description = validationException.InnerException?.Message;

                    if (validationError is DomainValidationError domainValidationError)
                        domainValidationError.Details = ((DomainException)ex).Details;

                    ((CustomValidationProblemDetails)problemDetails).Errors = new[] { validationError };

                    problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    problemDetails.Title = "Bad Request.";
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Detail = "One or more validation errors occurred.";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                }
            default:
                {
                    _logger.LogError(ex, $"An unhandled exception has occurred, {ex.Message}");
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                    problemDetails.Title = "Internal Server Error.";
                    problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Detail = "Internal server error occurred!";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
                }
        }

        context.Response.ContentType = "application/json";
        string result = JsonConvert.SerializeObject(problemDetails, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            SerializationBinder = ValidationErrorKnownTypes.GetKnownTypes()
        });
        await context.Response.WriteAsync(result);
    }
}