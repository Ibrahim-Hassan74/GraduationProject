using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.DTO.Microbus
{
    public class MicrobusQuery
    {
        public MicrobusSearchBy? SearchBy { get; set; }

        public string? SearchString { get; set; }

        public bool? IsActive { get; set; }

        public Guid? RouteId { get; set; }

        public Guid? DriverId { get; set; }

        public MicrobusSortBy? SortBy { get; set; }

        public SortOrderOptions OrderOptions { get; set; } = SortOrderOptions.ASC;
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
