using AutoMapper;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Report;
using System.Globalization;

namespace SmartMicrobus.Core.Mapping
{
    public class ReportMappingProfile : Profile
    {
        public ReportMappingProfile()
        {
            CreateMap<DriverReport, ReportResponse>()
                .ForMember(d => d.Status, opt => opt.MapFrom(sec => sec.Status.ToString()));

            CreateMap<DriverReport, ReportResponseWithDetails>()
                .ForMember(d => d.Reasons, opt => opt.MapFrom(src => src.Reasons.Select(r => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar" ? r.ReportReason.NameAr : r.ReportReason.NameEn).ToList()));

            CreateMap<DriverReport, ReportResponseForManager>()
                 .IncludeBase<DriverReport, ReportResponseWithDetails>()
                 .ForMember(d => d.PassengerName,
                     opt => opt.MapFrom(src => src.Passenger.ApplicationUser.DisplayName))
                 .ForMember(d => d.DriverName,
                     opt => opt.MapFrom(src => src.Driver.ApplicationUser.DisplayName))
                 .ForMember(d => d.PassangerId,
                     opt => opt.MapFrom(src => src.PassengerId))
                 .ForMember(d => d.DriverId,
                     opt => opt.MapFrom(src => src.DriverId));
        }
    }
}
