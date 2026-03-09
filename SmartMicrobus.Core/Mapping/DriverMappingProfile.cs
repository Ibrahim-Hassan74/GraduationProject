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
          

            CreateMap<QueueItem, DriverDashboardDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.RouteFrom, opt => opt.MapFrom(src => src.Queue.Route.FromEn))
                .ForMember(dest => dest.RouteTo, opt => opt.MapFrom(src => src.Queue.Route.ToEn));


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

        }
    }
}
