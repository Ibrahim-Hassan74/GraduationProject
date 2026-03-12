

using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.DTO.Queue
{
    public class TripHistoryDTO
    {
        public decimal Amount { get; set; }

        public string RouteFrom { get; set; }

        public string RouteTo { get; set; }

        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }

        public int PassengerCount { get; set; }

        public double Distance { get; set; }

        public TripStatus Status { get; set; }
    }
}
