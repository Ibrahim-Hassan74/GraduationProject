using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Driver
{
    public class DriverHistoryRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessageResourceName = "InvalidPageSize",
            ErrorMessageResourceType = typeof(Resources.DTO.Driver.DriverValidationMessages))]
        public int PageSize { get; set; } = 10;

        [Range(1, int.MaxValue,
            ErrorMessageResourceName = "InvalidPageNumber",
            ErrorMessageResourceType = typeof(Resources.DTO.Driver.DriverValidationMessages))]
        public int PageNumber { get; set; } = 1;
    }
}