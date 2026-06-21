using SmartMicrobus.Core.Helper;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Staff
{
    public class AddStaffDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [EgyptianPhone(ErrorMessageResourceName = "InvalidPhoneNumber",
                       ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;
    }
}
