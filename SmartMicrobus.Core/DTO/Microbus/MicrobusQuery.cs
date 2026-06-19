using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.DTO.Microbus
{
    public class MicrobusQuery
    {
        public string? Search { get; set; } // Search by PlateNumber, Model, or Color

        public MicrobusSortBy? SortBy { get; set; }

        public SortOrderOptions SortOrder { get; set; } = SortOrderOptions.ASC;

        public int? MinPassengerCount { get; set; }

        public int? MaxPassengerCount { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
