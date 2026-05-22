using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Station;
using System.Globalization;

namespace SmartMicrobus.Core.Mapping
{
    public class StationMappingProfile : Profile
    {
        public StationMappingProfile()
        {
            CreateMap<Station, StationResponse>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src =>
                        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                            ? src.NameAr
                            : src.NameEn))
                .ForMember(dest => dest.City,
                    opt => opt.MapFrom(src =>
                        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                            ? src.CityAr
                            : src.CityEn))
                .ForMember(dest => dest.Address,
                    opt => opt.MapFrom(src =>
                        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar"
                            ? src.AddressAr
                            : src.AddressEn))
                .ForMember(dest => dest.Lat,
                    opt => opt.MapFrom(src => src.Location.Y))
                .ForMember(dest => dest.Lng,
                    opt => opt.MapFrom(src => src.Location.X));
        }
    }
}
