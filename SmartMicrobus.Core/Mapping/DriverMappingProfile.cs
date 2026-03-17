using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.DTO.Trip;
using System.Globalization;

namespace SmartMicrobus.Core.Mapping
{
    public class DriverMappingProfile : Profile
    {
        public DriverMappingProfile()
        {
            //CreateMap<QueueItem, DriverDashboardDTO>()
            //    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            //    .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
            //    .ForMember(dest => dest.RouteFrom, opt => opt.MapFrom(src => src.Queue.Route != null ? src.Queue.Route.FromEn : null))
            //    .ForMember(dest => dest.RouteTo, opt => opt.MapFrom(src => src.Queue.Route.ToEn));

            //CreateMap<QueueItem, DriverDashboardDTO>()
            //   .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            //   .ForMember(dest => dest.Position,opt => opt.MapFrom(src => src.Position))
            //   .ForMember(dest => dest.RouteFrom,
            //       opt => opt.MapFrom(src =>
            //           CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
            //               ? src.Queue.Route.FromAr
            //               : src.Queue.Route.FromEn))
            //   .ForMember(dest => dest.RouteTo,
            //       opt => opt.MapFrom(src =>
            //           CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
            //               ? src.Queue.Route.ToAr
            //               : src.Queue.Route.ToEn));


            CreateMap<Trip, TripHistoryDTO>()
                .ForMember(dest => dest.RouteFrom,
                    opt => opt.MapFrom(src =>
                        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                            ? src.Route.FromAr
                            : src.Route.FromEn))

                .ForMember(dest => dest.RouteTo,
                    opt => opt.MapFrom(src =>
                        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                            ? src.Route.ToAr
                            : src.Route.ToEn))

                .ForMember(dest => dest.Distance,
                    opt => opt.MapFrom(src => src.DistanceKm))
                .ForMember(dest => dest.Amount,
                    opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.StartedAt,
                    opt => opt.MapFrom(src => src.StartedAt.ToString("yyyy MM dd hh:mm")))
                .ForMember(dest => dest.EndedAt,
                    opt => opt.MapFrom(src => src.EndedAt.HasValue ? src.EndedAt.Value.ToString("yyyy MM dd hh:mm") : null));

            //CreateMap<Trip, DriverDashboardDTO>()
            //    .ForMember(dest => dest.DriverId,
            //        opt => opt.MapFrom(src => src.DriverId))

            //    .ForMember(dest => dest.Status,
            //        opt => opt.MapFrom(src => src.Status.ToString()))

            //    .ForMember(dest => dest.RouteFrom,
            //        opt => opt.MapFrom(src =>
            //            CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
            //                ? src.Route.FromAr
            //                : src.Route.FromEn))

            //    .ForMember(dest => dest.RouteTo,
            //        opt => opt.MapFrom(src =>
            //            CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
            //                ? src.Route.ToAr
            //                : src.Route.ToEn));

            CreateMap<Trip, TripDashboardDTO>()
                .ForMember(dest => dest.TripId,
                    opt => opt.MapFrom(src => src.Id))

                .ForMember(dest => dest.RouteFrom,
                    opt => opt.MapFrom(src =>
                        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                            ? src.Route.FromAr
                            : src.Route.FromEn))

                .ForMember(dest => dest.RouteTo,
                    opt => opt.MapFrom(src =>
                        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                            ? src.Route.ToAr
                            : src.Route.ToEn))

                .ForMember(dest => dest.DistanceKm,
                    opt => opt.MapFrom(src => src.DistanceKm))

                .ForMember(dest => dest.StartedAt,
                    opt => opt.MapFrom(src => src.StartedAt))

                .ForMember(dest => dest.EstimatedArrivalMinutes,
                    opt => opt.MapFrom(src =>
                        (int)Math.Ceiling(src.DistanceKm * 1.2))); // will change it later

            CreateMap<Trip, MicrobusOnTheWayResponse>()
              .ForMember(dest => dest.DriverName,
                  opt => opt.MapFrom(src => src.Driver.ApplicationUser.DisplayName))
              .ForMember(dest => dest.Status,
                  opt => opt.MapFrom(src => src.Status.ToString()))

              .ForMember(dest => dest.PlateNumber,
                  opt => opt.MapFrom(src => src.Microbus.PlateNumber))
               .ForMember(dest => dest.PassengerCount,
                 opt => opt.MapFrom(src => src.Microbus.PassengerCount))
              .ForMember(dest => dest.Model,
                  opt => opt.MapFrom(src => src.Microbus.Model))
              .ForMember(dest => dest.Color,
                  opt => opt.MapFrom(src => src.Microbus.Color))

              .ForMember(dest => dest.Position,
                  opt => opt.Ignore())
              .ForMember(dest => dest.EstimatedArrivalMinutes,
                  opt => opt.Ignore());
        }
    }
}
