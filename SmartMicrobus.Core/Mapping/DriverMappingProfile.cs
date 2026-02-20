using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Queue;

namespace SmartMicrobus.Core.Mapping
{
    public class DriverMappingProfile : Profile
    {
        public DriverMappingProfile()
        {
            CreateMap<QueueItem, QueueItemDTO>()
                .ForMember(dest => dest.DriverId, opt => opt.MapFrom(src => src.DriverId))
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver != null ? src.Driver.ApplicationUser.DisplayName : ""))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.JoinedAt));

            CreateMap<QueueItem, DriverDashboardDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.RouteFrom, opt => opt.MapFrom(src => src.Queue.Route.FromEn))
                .ForMember(dest => dest.RouteTo, opt => opt.MapFrom(src => src.Queue.Route.ToEn));
        }
    }
}
