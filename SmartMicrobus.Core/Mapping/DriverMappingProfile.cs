using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Queue;
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

            CreateMap<QueueItem, DriverDashboardDTO>()
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
               .ForMember(dest => dest.Position,opt => opt.MapFrom(src => src.Position))
               .ForMember(dest => dest.RouteFrom,
                   opt => opt.MapFrom(src =>
                       CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                           ? src.Queue.Route.FromAr
                           : src.Queue.Route.FromEn))
               .ForMember(dest => dest.RouteTo,
                   opt => opt.MapFrom(src =>
                       CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                           ? src.Queue.Route.ToAr
                           : src.Queue.Route.ToEn));


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

            CreateMap<Trip, DriverDashboardDTO>()
                .ForMember(dest => dest.DriverId,
                    opt => opt.MapFrom(src => src.DriverId))

                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))

                .ForMember(dest => dest.RouteFrom,
                    opt => opt.MapFrom(src =>
                        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                            ? src.Route.FromAr
                            : src.Route.FromEn))

                .ForMember(dest => dest.RouteTo,
                    opt => opt.MapFrom(src =>
                        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                            ? src.Route.ToAr
                            : src.Route.ToEn));
        }
    }
}
