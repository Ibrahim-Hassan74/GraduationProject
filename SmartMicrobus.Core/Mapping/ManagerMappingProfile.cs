using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Manager;

namespace SmartMicrobus.Core.Mapping
{
    public class ManagerMappingProfile : Profile
    {
        public ManagerMappingProfile()
        {
            CreateMap<Manager, ManagerResponse>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.ApplicationUser.DisplayName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.ApplicationUser.PhoneNumber))
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Station.NameEn)); // Default to En or handle localization

            CreateMap<Microbus, SmartMicrobus.Core.DTO.Microbus.MicrobusResponse>();
        }
    }
}
