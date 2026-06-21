using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Admin
{
    public class UpdateManagerStationDTO
    {
        [Required(ErrorMessage = "Station Id is required")]
        public Guid StationId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}
