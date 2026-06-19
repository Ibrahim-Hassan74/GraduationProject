using System.Text.Json.Serialization;

namespace SmartMicrobus.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MicrobusSearchBy
    {
        PlateNumber,
        DriverName,
        Model,
        Color,
        Route
    }
}
