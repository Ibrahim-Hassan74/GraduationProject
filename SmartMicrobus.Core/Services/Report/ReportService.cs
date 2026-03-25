using AutoMapper;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Report;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Report;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace SmartMicrobus.Core.Services.Report
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportRepository _reportRepository;
        private readonly IReportReasonRepository _reasonRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ReportService> _localizer;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<ReportService> localizer)
        {
            _unitOfWork = unitOfWork;
            _reportRepository = unitOfWork.ReportRepository;
            _reasonRepository = unitOfWork.ReportReasonRepository;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<ApiResponse> CreateReportAsync(Guid passengerId, CreateReportRequest request)
        {
           var validate= ValidationHelper.ModelValidation(request);
            if(!validate.Success)
                return validate;

            var driver = await _unitOfWork.MicrobusRepository.GetDriverAsync(request.PlateNumber);
            if (driver is null)
                return ApiResponseFactory.NotFound(
                    _localizer["Report_Invalid_Plate"]
                );

            var recentReports = await _reportRepository.HasRecentReportAsync(passengerId, request.PlateNumber);

            if (recentReports)
                return ApiResponseFactory.Conflict(
                    _localizer["Report_Already_Submitted"]
                );

            var reasons = await _reasonRepository.GetByIdsAsync(request.ReasonIds);
            if (reasons == null || reasons.Count != request.ReasonIds.Count)
                return ApiResponseFactory.BadRequest(
                    _localizer["Report_Invalid_Reasons"]
                );

            var report = new DriverReport
            {
                Id = Guid.NewGuid(),
                PassengerId = passengerId,
                DriverId = driver?.Id,
                PlateNumber = request.PlateNumber,
                Description = request.Description,
                CreatedAt = DateTimeOffset.UtcNow
            };

            foreach (var reason in reasons)
            {
                report.Reasons.Add(new DriverReportReason
                {
                    Id = Guid.NewGuid(),
                    DriverReportId = report.Id,
                    ReportReasonId = reason.Id
                });
            }

            await _reportRepository.AddAsync(report);
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success(
                _localizer["Report_Submitted_Success"]
            );
        }

        public async Task<ApiResponse> GetReasonsAsync()
        {
            var reasons = await _reasonRepository.GetAllAsync();
            if (reasons == null || !reasons.Any())
                return ApiResponseFactory.NotFound(
                    _localizer["Report_Reasons_Not_Found"]
                );

            var list = reasons.Select(r => new ReportReasonResponse
            {
                Id = r.Id,
                Name = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar" ? r.NameAr : r.NameEn
            }).ToList();

            return ApiResponseFactory.Success(
                _localizer["Report_Reasons_Retrieved"],
                list
            );
        }
    }
}
