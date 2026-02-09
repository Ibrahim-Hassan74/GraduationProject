using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SmartMicrobus.Core.Helper
{
    public class EgyptianPhoneAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var phoneNumber = value as string;

            var normalizedPhone = NormalizePhone(phoneNumber);
            if (normalizedPhone is null)
                return new ValidationResult("Phone number must be a valid Egyptian number starting with +20 or 20 and contain 10 digits after the country code.");

            var property = validationContext.ObjectType
            .GetProperty(validationContext.MemberName!);

            property?.SetValue(validationContext.ObjectInstance, normalizedPhone);

            return ValidationResult.Success;

        }

        private static string? NormalizePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;

            var cleaned = Regex.Replace(phone, @"\D", "");

            if (cleaned.Length < 10)
                return null;

            if (cleaned.StartsWith("20"))
                cleaned = cleaned[2..];

            if (!cleaned.StartsWith("1") || cleaned.Length != 10)
                return null;

            return "20" + cleaned;
        }
    }
}
