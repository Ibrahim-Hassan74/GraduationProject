using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Staff
{
    public class AddStaffDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;
    }
}
