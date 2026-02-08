using SmartMicrobus.Core.DTO.Common;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.Helper
{
    public class ValidationHelper
    {
        internal static ApiResponse ModelValidation(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(obj, validationContext, results, true);
            if(!isValid)
            {
                return ApiResponseFactory.Failure(results.FirstOrDefault()?.ErrorMessage, 400, results.Select(x => x.ErrorMessage)?.ToArray());
            }
            return ApiResponseFactory.Success("Validation successful");
        }
    }
}
