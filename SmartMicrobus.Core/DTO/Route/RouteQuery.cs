using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.DTO.Route
{
    public class RouteQuery
    {
        public string? Search { get; set; }

        public RouteSortBy? SortBy { get; set; }

        public SortOrderOptions SortOrder { get; set; } = SortOrderOptions.ASC;

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public double? MinDistance { get; set; }

        public double? MaxDistance { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
