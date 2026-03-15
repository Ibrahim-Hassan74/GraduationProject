namespace SmartMicrobus.Core.DTO.Route
{
    public class RouteSummaryResponse
    {
        public decimal Price { get; set; }
        public double DistanceKm { get; set; }
        public int NumberOfMicrobusesInQueue { get; set; }
        public int NumberOfMicrobusesOnTheWay { get; set; }
        public int NearestArrivalMinutes { get; set; }
    }
}
