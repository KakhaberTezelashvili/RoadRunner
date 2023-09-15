using TDOC.Common.Binders;
using TDOC.Common.Data.Models.Errors;

namespace TDOC.Common.Data.Model.Errors
{
    public static class ValidationErrorKnownTypes
    {
        public static KnownTypesBinder GetKnownTypes()
        {
            return new KnownTypesBinder
            {
                KnownTypes = new List<Type>
                {
                    typeof(ValidationError),
                    typeof(ValidationError[]),
                    typeof(DomainValidationError),
                    typeof(DomainValidationError[]),
                    typeof(InputArgumentValidationError),
                    typeof(InputArgumentValidationError[]),
                    typeof(ValidationCodeDetails),
                    typeof(List<ValidationCodeDetails>)
                }
            };
        }
    }
}