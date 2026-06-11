using NetTopologySuite.Geometries;

namespace SmartMicrobus.Core.DTO.Station
{
    public class StationAddRequest
    {
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

        public string CityAr { get; set; } = null!;
        public string CityEn { get; set; } = null!;

        public bool IsActive { get; set; } = true;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? AddressAr { get; set; }
        public string? AddressEn { get; set; }
        public Point? Location { get; set; } = null!;
    }
}
