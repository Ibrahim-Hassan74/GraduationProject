using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMicrobus.Core.DTO.Driver
{
    public class EarningEstimationResponse
    {
        public Guid driverId { get; set; }
        public Guid routeId { get; set; }
        public decimal AvgDailyEarning { get; set; }
        public decimal AvgTripsPerDay { get; set; }
        public int WorkingDaysCount { get; set; }
        public decimal EarningPerTrip { get; set; }
        public string DataSource { get; set; }
        public List<DayBreakdown> DayBreakdowns { get; set; }
    }
}
