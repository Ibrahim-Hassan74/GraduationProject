using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Driver;

namespace SmartMicrobus.Core.Services.Drivers
{
    public class EarningService(ITripRepository tripRepository, IDriverRepository driverRepository) : IEarningService
    {
        public async Task<ApiResponse> GetDailyEarningEstimationAsync(Guid driverId, Guid routeId, Guid stationId)
        {
            var yesterday = DateTime.UtcNow.AddDays(-1);
            var request = new DriverHistoryRequest
            {
                FromDate = DateTime.MinValue,
                ToDate = yesterday,
            };

            var trips = (await tripRepository.GetDriverTripsAsync(driverId, request)).Trips;

            if (trips is not null && trips.Count != 0)
            {
                var earningPerDay = trips.
                    GroupBy(t => t.StartedAt)
                    .Select(g => new DayBreakdown
                    {
                        Date = g.Key,
                        TripsCount = g.Count(),
                        DayEarning = g.Sum(t => t.TotalAmount)
                    })
                    .OrderByDescending(d => d.Date)
                    .ToList();

                var averageDailyEarning = earningPerDay.Average(e => e.DayEarning);
                var averageTripsPerDay = earningPerDay.Average(e => e.TripsCount);

                var estimationResult = new EarningEstimationResponse
                {
                    driverId = driverId,
                    routeId = routeId,
                    AvgDailyEarning = averageDailyEarning,
                    AvgTripsPerDay = averageTripsPerDay,
                    WorkingDaysCount = earningPerDay.Count,
                    EarningPerTrip = averageDailyEarning / averageTripsPerDay,
                    DataSource = "Historical Driver Trips",
                    DayBreakdowns = earningPerDay
                };
                return ApiResponseFactory.Success("Earnings estimation retrieved successfully.", estimationResult);
            }

            var stationAvgResult = await GetStationAvgEarningsAsync(
            driverId, routeId, stationId, yesterday);

            if (stationAvgResult != null)
            {
                return ApiResponseFactory.Success("Earnings estimation retrieved successfully using station averages.", stationAvgResult);
            }

            return ApiResponseFactory.NotFound("No trip data available for estimation.");
        }

        private async Task<EarningEstimationResponse?> GetStationAvgEarningsAsync(Guid driverId, Guid routeId, Guid stationId, DateTime referenceDate)
        {
            var stationHistoryResponse = await tripRepository.GetStationTripsAsync(routeId, stationId);

            if (stationHistoryResponse == null)
                return null;

            var averageDailyEarning = stationHistoryResponse
                .GroupBy(t => new
                {
                    t.Microbus.DriverId, t.StartedAt.Date
                })
                .Select(g => g.Sum(t => t.TotalAmount))
                .Average();

            var avgTripsPerDay = stationHistoryResponse
            .GroupBy(t => new { t.Microbus.DriverId, t.StartedAt.Date })
            .Select(g => (decimal)g.Count())
            .Average();

            var driver = await driverRepository.GetByIdAsync(driverId, d => d.Microbus);

            var Microbus = driver?.Microbus;
            var Route = Microbus?.Route;

            var earningPerTrip = (Microbus != null && Route != null)
            ? Microbus.PassengerCount * Route.Price
            : stationHistoryResponse.Average(t => t.TotalAmount);

            var adjustedAvgDailyEarning = avgTripsPerDay * earningPerTrip;

            return new EarningEstimationResponse
            {
                driverId = driverId,
                routeId = routeId,
                AvgDailyEarning = Math.Round(adjustedAvgDailyEarning, 2),
                AvgTripsPerDay = Math.Round(avgTripsPerDay, 1),
                WorkingDaysCount = 0,
                EarningPerTrip = (decimal)earningPerTrip,
                DataSource = "Station Average",
                DayBreakdowns = new List<DayBreakdown>()
            };
        }
    }
}
