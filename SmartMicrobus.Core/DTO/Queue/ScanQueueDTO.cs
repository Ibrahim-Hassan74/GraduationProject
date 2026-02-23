using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Queue
{
    public class ScanQueueDTO
    {
        [Required]
        public Guid MicrobusId { get; set; }

        [Required]
        public Guid StationId { get; set; }

    }
}