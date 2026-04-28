using AutoMapper;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Localization;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.DTO.Station;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Route;
using SmartMicrobus.Core.ServiceContracts.Stations;
using System.Globalization;

namespace SmartMicrobus.Core.Services.Stations
{
    public class StationsService : IStationsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOsrmRouteService _osrmRouteService;
        private readonly IStringLocalizer<StationsService> _localizer;
        private readonly IMapper _mapper;

        public StationsService(IUnitOfWork unitOfWork, IOsrmRouteService osrmRouteService, IStringLocalizer<StationsService> localizer, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _osrmRouteService = osrmRouteService;
            _localizer = localizer;
            _mapper = mapper;
        }

        public async Task<ApiResponse> GetNearestStationAsync(double lat, double lng, TransportMode mode)
        {
            var station = await _unitOfWork.StationRepository
                .GetNearestStationAsync(lat, lng);

            if (station == null)
                throw new Exception(_localizer["NoStationsFound"]);

            var route = await _osrmRouteService.GetRouteAsync(new RouteRequest
            {
                StartLat = lat,
                StartLng = lng,
                EndLat = station.Location.Y,
                EndLng = station.Location.X,
                TransportMode = mode,
            });

            var result = new NearestStationResponse
            {
                StationId = station.Id,
                StationName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar" ? station.NameAr : station.NameEn,
                StationAddress = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar" ? station.AddressAr : station.AddressEn,
                StationCity = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar" ? station.CityAr : station.CityEn,
                StationLat = station.Location.Y,
                StationLng = station.Location.X,
                DistanceKm = route.Distance,
                EtaMinutes = route.Duration,
                Points = route.Coordinates
            };

            return ApiResponseFactory.Success(_localizer["NearestStationFound"], result);
        }

        public async Task<ApiResponse> GetAllStationsAsync()
        {
            var stations = await _unitOfWork.StationRepository.GetAllAsync();

            var stationDtos = _mapper.Map<IEnumerable<StationResponse>>(stations);
            return ApiResponseFactory.Success(_localizer["StationsFound"], stationDtos);
        }

        public async Task<ApiResponse> GetStationByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(_localizer["InvalidStationId"]);

            var station = await _unitOfWork.StationRepository.GetByIdAsync(id);

            if (station == null)
                return ApiResponseFactory.NotFound(_localizer["StationNotFound"]);

            return ApiResponseFactory.Success(_localizer["StationFound"], _mapper.Map<StationResponse>(station));
        }

        public async Task<ApiResponse> GetStationDetailsWithRouteAsync(Guid stationId, double userLat, double userLng, TransportMode mode)
        {
            var station = await _unitOfWork.StationRepository.GetByIdAsync(stationId);

            if (station == null)
                return ApiResponseFactory.NotFound(_localizer["StationNotFound"]);

            var route = await _osrmRouteService.GetRouteAsync(new RouteRequest
            {
                StartLat = userLat,
                StartLng = userLng,
                EndLat = station.Location.Y,
                EndLng = station.Location.X,
                TransportMode = mode
            });

            var isArabic = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";

            var response = new StationDetailsWithRouteResponse
            {
                Id = station.Id,

                Name = isArabic ? station.NameAr : station.NameEn,
                City = isArabic ? station.CityAr : station.CityEn,
                Address = isArabic ? station.AddressAr : station.AddressEn,

                Lat = station.Location.Y,
                Lng = station.Location.X,

                DistanceKm = route.Distance,
                EtaMinutes = route.Duration,

                Points = route.Coordinates
            };

            return ApiResponseFactory.Success(_localizer["StationDetailsWithRouteFound"], response);
        }

        public async Task<ApiResponse> GetRouteBetweenStationsAsync(Guid fromStationId, Guid toStationId, TransportMode mode)
        {
            var fromStation = await _unitOfWork.StationRepository.GetByIdAsync(fromStationId);
            var toStation = await _unitOfWork.StationRepository.GetByIdAsync(toStationId);

            if (fromStation == null || toStation == null)
                return ApiResponseFactory.NotFound(_localizer["StationsNotFound"]);

            var route = await _osrmRouteService.GetRouteAsync(new RouteRequest
            {
                StartLat = fromStation.Location.Y,
                StartLng = fromStation.Location.X,
                EndLat = toStation.Location.Y,
                EndLng = toStation.Location.X,
                TransportMode = mode
            });

            var isArabic = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";

            var result = new RouteBetweenStationsResponse
            {
                FromStationId = fromStation.Id,
                FromName = isArabic ? fromStation.NameAr : fromStation.NameEn,
                FromLat = fromStation.Location.Y,
                FromLng = fromStation.Location.X,

                ToStationId = toStation.Id,
                ToName = isArabic ? toStation.NameAr : toStation.NameEn,
                ToLat = toStation.Location.Y,
                ToLng = toStation.Location.X,

                DistanceKm = route.Distance,
                EtaMinutes = route.Duration,

                Points = route.Coordinates
            };

            return ApiResponseFactory.Success(_localizer["RouteBetweenStationsFound"], result);
        }
    }
}
