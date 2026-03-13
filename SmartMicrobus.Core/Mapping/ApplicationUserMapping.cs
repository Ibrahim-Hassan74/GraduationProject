using AutoMapper;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Account;

namespace SmartMicrobus.Core.Mapping
{
    public class ApplicationUserMapping : Profile
    {
        public ApplicationUserMapping()
        {
            CreateMap<ApplicationUser, ApplicationUserResponse>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(src => src.LockoutEnd == null || src.LockoutEnd < DateTime.UtcNow))
                .ForMember(dest => dest.IsConfirmed, opt => opt.MapFrom(src => src.PhoneNumberConfirmed))
                .ForMember(dest => dest.PhotoUrl,
                    opt => opt.MapFrom(src => src.Photo != null ? src.Photo.ImageName : ""));
        }
    }
}
