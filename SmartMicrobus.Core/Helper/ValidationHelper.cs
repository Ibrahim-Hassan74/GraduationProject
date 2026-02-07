using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.Helper
{
    public class ValidationHelper
    {
        internal static void ModelValidation(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(obj, validationContext, results, true);
            if (!isValid)
                throw new ArgumentException(results.FirstOrDefault()?.ErrorMessage);
        }
    }
}
