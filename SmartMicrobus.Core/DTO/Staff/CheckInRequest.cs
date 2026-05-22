using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Staff
{
    public class CheckInRequest
    {
        [Required(ErrorMessageResourceName = "RequiredQrCode",
                  ErrorMessageResourceType = typeof(Resources.DTO.Staff.StaffValidationMessages))]
        public string QrCode { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = "RequiredStationId",
                  ErrorMessageResourceType = typeof(Resources.DTO.Staff.StaffValidationMessages))]
        public Guid StationId { get; set; }
    }
}