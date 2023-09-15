using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;
using TDOC.Common.Data.Enumerations.Errors;

namespace TDOC.Common.Server.Validations
{
    public class CustomValidationProblemDetails : ValidationProblemDetails
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public new string Detail { get; set; }

        [JsonPropertyName("errors")]
        public new IEnumerable<ValidationError> Errors { get; set; }

        public CustomValidationProblemDetails(IEnumerable<ValidationError> errors = null)
        {
            Errors = errors;
        }

        public CustomValidationProblemDetails(ModelStateDictionary modelState)
        {
            Errors = ConvertModelStateErrorsToValidationErrors(modelState);
        }

        private static IEnumerable<ValidationError> ConvertModelStateErrorsToValidationErrors(ModelStateDictionary modelStateDictionary)
        {
            List<ValidationError> validationErrors = new();
            foreach (ModelErrorCollection errors in modelStateDictionary.Select(keyModelStatePair => keyModelStatePair.Value.Errors).Where(errors => errors.Count > 0))
            {
                foreach (ModelError error in errors)
                {
                    if (int.TryParse(error.ErrorMessage, out int code))
                        validationErrors.Add(new ValidationError { Code = (ModelStateErrorCodes)code });
                    else
                        validationErrors.Add(new ValidationError { Message = error.ErrorMessage });
                }
            }
            return validationErrors;
        }
    }
}