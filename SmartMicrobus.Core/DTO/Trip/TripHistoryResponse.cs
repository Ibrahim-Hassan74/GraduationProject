using Trips = SmartMicrobus.Core.Domain.Entities.Trip;

namespace SmartMicrobus.Core.DTO.Trip
{
    public class TripHistoryResponse
    {
        public decimal TotalAmount { get; set; }
        public List<Trips> Trips { get; set; }
        public int TotalCount { get; set; }
        public TripHistoryResponse(decimal totalAmount, List<Trips> trips, int totalCount)
        {
            TotalAmount = totalAmount;
            Trips = trips;
            TotalCount = totalCount;
        }
    }
}
