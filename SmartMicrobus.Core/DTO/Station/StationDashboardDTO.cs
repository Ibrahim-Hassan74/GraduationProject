namespace SmartMicrobus.Core.DTO.Station
{
    public class StationDashboardDTO
    {
        public int AvailableMicrobuses { get; set; }
        public int IncomingMicrobuses { get; set; }
        public int CompletedTripsToday { get; set; }
        public int TotalPassengersToday { get; set; }

        public List<RouteDemandDTO> DemandByRoute { get; set; } = new();
        public List<HourlyTripVolumeDTO> TripsOverTime { get; set; } = new();
        public List<LiveRouteQueueDTO> LiveRouteQueues { get; set; } = new();
    }

    public class RouteDemandDTO
    {
        public string DestinationName { get; set; } = null!;
        public int PassengerCount { get; set; }
    }

    public class HourlyTripVolumeDTO
    {
        public string Hour { get; set; } = null!;
        public int TripCount { get; set; }
    }

    public class LiveRouteQueueDTO
    {
        public string DestinationName { get; set; } = null!;
        public int MicrobusesReady { get; set; }
    }
}
