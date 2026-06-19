namespace SmartMicrobus.Core.DTO.Route
{
    public class RouteLiveUpdateDTO
    {
        public int NumberOfMicrobusesInQueue { get; set; }

        public int NumberOfMicrobusesOnTheWay { get; set; }

        public int? NearestArrivalMinutes { get; set; }
    }
}
