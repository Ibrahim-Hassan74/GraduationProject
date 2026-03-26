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

        }
    }
}
