using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMicrobus.Core.DTO.Driver
{
    public class DayBreakdown
    {
        public DateTimeOffset Date { get; set; }
        public decimal TripsCount { get; set; }
        public decimal DayEarning { get; set; }
    }
}
