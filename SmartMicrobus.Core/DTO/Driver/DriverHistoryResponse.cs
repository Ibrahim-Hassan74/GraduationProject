using SmartMicrobus.Core.DTO.Queue;

namespace SmartMicrobus.Core.DTO.Driver
{
    public class DriverHistoryResponse
    {
        public decimal TotalAmount { get; set; }
        public List<TripHistoryDTO> Trips { get; set; } = new List<TripHistoryDTO>();
        public int TotalCount { get; set; }
        public DriverHistoryResponse(decimal totalAmount, List<TripHistoryDTO> trips, int totalCount)
        {
            TotalAmount = totalAmount;
            Trips = trips;
            TotalCount = totalCount;
        }
        public DriverHistoryResponse() { }
    }
}
