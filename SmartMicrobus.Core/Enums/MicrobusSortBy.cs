using System.Text.Json.Serialization;

namespace SmartMicrobus.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MicrobusSortBy
    {
        PlateNumber,
        DriverName,
        PassengerCount,
        RouteName,
        Model,
        Color
    }
}
