using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    opt => opt.MapFrom(src => src.Status.ToString()));


        }
    }
}
