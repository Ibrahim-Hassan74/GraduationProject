using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SmartMicrobus.Core.Helper
{
    public class EgyptianPhoneAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string phoneNumber || string.IsNullOrWhiteSpace(phoneNumber))
            {
                return ValidationResult.Success; // required attribute do this validation
            }

            var normalizedPhone = NormalizePhone(phoneNumber);

            if (normalizedPhone is null)
            {
                return new ValidationResult(GetErrorMessage(validationContext));
            }

            SetNormalizedValue(validationContext, normalizedPhone);

            return ValidationResult.Success;
        }

        private static string? NormalizePhone(string phone)
        {
            var cleaned = Regex.Replace(phone, @"\D", "");

            if (string.IsNullOrEmpty(cleaned))
                return null;

            var last9Digits = cleaned.Length >= 9 ? cleaned[^9..] : null;

            if (last9Digits is null || last9Digits.Length != 9)
                return null;

            return $"201{last9Digits}";
        }

        private static void SetNormalizedValue(ValidationContext context, string normalizedValue)
        {
            var property = context.ObjectType.GetProperty(context.MemberName!);

            if (property != null && property.CanWrite)
            {
                property.SetValue(context.ObjectInstance, normalizedValue);
            }
        }

        private string GetErrorMessage(ValidationContext context)
        {
            if (!string.IsNullOrEmpty(ErrorMessageResourceName))
            {
                return FormatErrorMessage(context.DisplayName);
            }

            return ErrorMessage ??
                   "Invalid Egyptian phone number format.";
        }
    }
}