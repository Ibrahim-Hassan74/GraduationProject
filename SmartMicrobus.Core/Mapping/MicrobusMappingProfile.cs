using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Microbus;

namespace SmartMicrobus.Core.Mapping
{
    public class MicrobusMappingProfile : Profile
    {
        public MicrobusMappingProfile()
        {
            CreateMap<Microbus, MicrobusListResponse>()
                .ForMember(d => d.DriverName,
                    o => o.MapFrom(src => GetDriverName(src)))
                .ForMember(d => d.RouteName,
                    o => o.MapFrom(src => GetRouteName(src)));

            CreateMap<Microbus, MicrobusDetailsResponse>()
                .ForMember(d => d.DriverName,
                    o => o.MapFrom(src => GetDriverName(src)))
                .ForMember(d => d.RouteName,
                    o => o.MapFrom(src => GetRouteName(src)));
        }

        private static string? GetDriverName(Microbus src)
        {
            return src.Driver?.ApplicationUser?.DisplayName;
        }

        private static string GetRouteName(Microbus src)
        {
            return $"{src.Route.FromAr} - {src.Route.ToAr}";
        }
    }
}
