using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.DTO.Driver
{
    public class DriverQuery
    {
        public string? Search { get; set; } // By DriverName, LicenseNumber, PlateNumber

        public DriverSortBy? SortBy { get; set; }

        public SortOrderOptions SortOrder { get; set; } = SortOrderOptions.ASC;

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
