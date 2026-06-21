using SmartMicrobus.Core.Helper;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Staff
{
    public class UpdateStaffDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [EgyptianPhone(ErrorMessageResourceName = "InvalidPhoneNumber",
                       ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string PhoneNumber { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
