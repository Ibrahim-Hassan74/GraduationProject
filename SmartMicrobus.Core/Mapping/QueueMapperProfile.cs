using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Queue;
using SmartMicrobus.Core.DTO.Route;
using System.Globalization;

namespace SmartMicrobus.Core.Mapping
{
    public class QueueMapperProfile : Profile
    {
        public QueueMapperProfile()
        {


            CreateMap<QueueItem, QueueItemDTO>()
                .ForMember(dest => dest.DriverId, opt => opt.MapFrom(src => src.DriverId))
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver != null && src.Driver.ApplicationUser != null ? src.Driver.ApplicationUser.DisplayName : ""))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.JoinedAt));


            CreateMap<QueueItem, QueueItemResponse>()
                .ForMember(dest => dest.DriverName,
                    opt => opt.MapFrom(src =>
                        src.Driver != null
                            ? src.Driver.ApplicationUser.DisplayName
                            : ""))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PlateNumber,
                    opt => opt.MapFrom(src =>
                        src.Microbus != null
                            ? src.Microbus.PlateNumber
                            : ""));


            CreateMap<QueueItem, QueueDashboardDTO>()
                .ForMember(dest => dest.QueueId,
                    opt => opt.MapFrom(src => src.QueueId))

                .ForMember(dest => dest.Position,
                    opt => opt.MapFrom(src => src.Position))

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

        }
    }
}
