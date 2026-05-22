using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.DTO.Route;
using System.Globalization;

namespace SmartMicrobus.Core.Mapping
{
    public class RouteMappingProfile : Profile
    {
        public RouteMappingProfile()
        {

            CreateMap<Route, DestinationResponse>()
               .ForMember(dest => dest.To,
                   opt => opt.MapFrom(src =>
                       CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                           ? src.ToAr
                           : src.ToEn))
               .ForMember(dest => dest.RouteId,
                   opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.StationId,
                   opt => opt.MapFrom(src => src.ToStationId));


            CreateMap<FavoriteRoute, FavoriteRouteResponse>()
            .ForMember(dest => dest.From,
                opt => opt.MapFrom(src =>
                    CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                        ? src.Route.FromAr
                        : src.Route.FromEn))

            .ForMember(dest => dest.To,
                opt => opt.MapFrom(src =>
                    CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                        ? src.Route.ToAr
                        : src.Route.ToEn))

            .ForMember(dest => dest.Price,
                opt => opt.MapFrom(src => src.Route.Price));


            CreateMap<RouteAddRequest, Route>();
            CreateMap<RouteUpdateRequest, Route>();
            CreateMap<Route, RouteResponse>();


          

        }
    }
}
