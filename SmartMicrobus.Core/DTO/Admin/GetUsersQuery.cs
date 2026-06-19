using SmartMicrobus.Core.Enums;

namespace SmartMicrobus.Core.DTO.Admin
{
    public class GetUsersQuery
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public UserRole? Role { get; set; }
        public UsersSortBy? SortBy { get; set; } = UsersSortBy.Name;
        public SortOrderOptions SortOrder { get; set; } = SortOrderOptions.ASC;
    }
}
