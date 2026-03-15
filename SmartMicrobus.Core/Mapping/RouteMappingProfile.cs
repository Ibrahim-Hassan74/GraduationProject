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
                   opt => opt.MapFrom(src => src.Id));

            CreateMap<QueueItem, MicrobusAtStationResponse>()
               .ForMember(dest => dest.DriverName,
                   opt => opt.MapFrom(src => src.Driver.ApplicationUser.DisplayName))
               .ForMember(dest => dest.DriverId,
                   opt => opt.MapFrom(src => src.Driver.ApplicationUser.Id))
               .ForMember(dest => dest.PlateNumber,
                   opt => opt.MapFrom(src => src.Microbus.PlateNumber))
               .ForMember(dest => dest.Status,
                   opt => opt.MapFrom(src => src.Status.ToString()))
               .ForMember(dest => dest.PassengerCount,
                   opt => opt.MapFrom(src => src.Microbus.PassengerCount))
               .ForMember(dest => dest.Model,
                   opt => opt.MapFrom(src => src.Microbus.Model))
               .ForMember(dest => dest.Color,
                   opt => opt.MapFrom(src => src.Microbus.Color));

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
