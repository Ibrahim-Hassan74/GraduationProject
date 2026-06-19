using System.Text.Json.Serialization;

namespace SmartMicrobus.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RouteSortBy
    {
        To = 1,
        Price = 2,
        Distance = 3
    }
}
