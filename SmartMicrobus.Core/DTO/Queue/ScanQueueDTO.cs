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

    public record CheckInRequest(string QrCode, Guid StationId);
    public record CheckOutRequest(string QrCode);
}