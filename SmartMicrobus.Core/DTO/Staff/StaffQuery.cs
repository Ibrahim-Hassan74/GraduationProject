using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.DTO.Staff
{
    public class StaffQuery
    {
        public string? Search { get; set; } // By Name or PhoneNumber
        public SortOrderOptions SortOrder { get; set; } = SortOrderOptions.ASC;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
