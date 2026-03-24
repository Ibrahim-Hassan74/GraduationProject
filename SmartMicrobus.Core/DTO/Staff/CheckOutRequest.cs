using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Staff
{
    public record CheckOutRequest
    {
        [Required(ErrorMessageResourceName = "RequiredQrCode",
                  ErrorMessageResourceType = typeof(Resources.DTO.Staff.StaffValidationMessages))]
        public string QrCode { get; set; } = string.Empty;
    }

}