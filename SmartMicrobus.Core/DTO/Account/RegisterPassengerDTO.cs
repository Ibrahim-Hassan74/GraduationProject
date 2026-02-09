using SmartMicrobus.Core.Helper;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class RegisterPassengerDTO
    {
        public string Name { get; set; }
        [EgyptianPhone]
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

    }
}
