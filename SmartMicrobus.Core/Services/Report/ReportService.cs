using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Report;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Common;
using SmartMicrobus.Core.ServiceContracts.Report;
using System.Globalization;

namespace SmartMicrobus.Core.Services.Report
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportRepository _reportRepository;
        private readonly IReportReasonRepository _reasonRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ReportService> _localizer;
        private readonly ICustomWhatsAppService _customWhatsAppService;
        private readonly UserManager<ApplicationUser> _userManager;
        public ReportService(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<ReportService> localizer, ICustomWhatsAppService customWhatsAppService, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _reportRepository = unitOfWork.ReportRepository;
            _reasonRepository = unitOfWork.ReportReasonRepository;
            _mapper = mapper;
            _localizer = localizer;
            _customWhatsAppService = customWhatsAppService;
            _userManager = userManager;
        }

        public async Task<ApiResponse> GetAllReportsAsync(GetReportsQuery query, Guid stationId)
        {

            var (items, totalCount) = await _reportRepository.GetPagedReportsForAdminAsync(query, stationId);

            var reportResponses = _mapper.Map<List<ReportResponse>>(items);

            var pagedResponse = new PagedResponse<ReportResponse>
            {
                Items = reportResponses,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            return ApiResponseFactory.Success(_localizer["ReportsRetrieved"], pagedResponse);
        }

        public async Task<ApiResponse> GetReportByIdForAdminAsync(Guid reportId, Guid stationId)
        {
            var report = await _reportRepository.GetByIdWithReasonsAsync(reportId, stationId);

            if (report == null)
                return ApiResponseFactory.NotFound(_localizer["ReportNotFound"]);

            var reportResponse = _mapper.Map<ReportResponseForManager>(report);

            return ApiResponseFactory.Success(_localizer["ReportRetrieved"], reportResponse);
        }

        public async Task<ApiResponse> UpdateReportStatusAsync(Guid reportId, UpdateReportStatusRequest request)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);

            if (report == null)
                return ApiResponseFactory.NotFound(_localizer["ReportNotFound"]);

            report.Status = request.Status;
            report.ResolvedAt = request.Status == ReportStatus.Reviewed ? DateTimeOffset.UtcNow : (DateTimeOffset?)null;

            await _reportRepository.UpdateAsync(report);
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success(_localizer["ReportUpdated"]);
        }

        public async Task<ApiResponse> CreateReportAsync(Guid passengerId, CreateReportRequest request)
        {
            var validate = ValidationHelper.ModelValidation(request);
            if (!validate.Success)
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
                CreatedAt = DateTimeOffset.UtcNow,
                Status = ReportStatus.Pending
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
            var user = await _userManager.FindByIdAsync(passengerId.ToString());
            if (user != null)
            {
                BackgroundJob.Enqueue(() =>
                    _customWhatsAppService.SendMessageAsync(
                        user.PhoneNumber!,
                        _localizer["Report_Submitted_Message"]
                    )
                );
            }

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

        public async Task<ApiResponse> GetReportsAsync(Guid passengerId, GetReportsQuery query)
        {
            var (items, totalCount) = await _reportRepository.GetPagedReportsAsync(passengerId, query);

            var reportResponses = _mapper.Map<List<ReportResponse>>(items);

            var pagedResponse = new PagedResponse<ReportResponse>
            {
                Items = reportResponses,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            return ApiResponseFactory.Success(_localizer["ReportsRetrieved"], pagedResponse);
        }

        public async Task<ApiResponse> GetReportByIdAsync(Guid passengerId, Guid reportId)
        {
            var report = await _reportRepository.GetByIdWithReasonsAsync(reportId);

            if (report == null)
                return ApiResponseFactory.NotFound(_localizer["ReportNotFound"]);

            if (report.PassengerId != passengerId)
                return ApiResponseFactory.Failure(_localizer["UnauthorizedAccessReport"], 403);

            var reportResponse = _mapper.Map<ReportResponseWithDetails>(report);

            return ApiResponseFactory.Success(_localizer["ReportRetrieved"], reportResponse);
        }

        public async Task<ApiResponse> UpdateReportAsync(Guid passengerId, Guid reportId, UpdateReportRequest request)
        {
            var validate = ValidationHelper.ModelValidation(request);
            if (!validate.Success)
                return validate;

            var report = await _reportRepository.GetByIdAsync(reportId, x => x.Reasons);

            if (report == null)
                return ApiResponseFactory.NotFound(_localizer["ReportNotFound"]);

            if (report.PassengerId != passengerId)
                return ApiResponseFactory.Failure(_localizer["UnauthorizedUpdateReport"], 403);

            if (report.Status != ReportStatus.Pending)
                return ApiResponseFactory.Conflict(_localizer["ReportUpdateOnlyPending"]);

            var driver = await _unitOfWork.MicrobusRepository.GetDriverAsync(request.PlateNumber);
            if (driver is null)
                return ApiResponseFactory.BadRequest(_localizer["InvalidPlateNumber"]);

            var reasons = await _reasonRepository.GetByIdsAsync(request.ReasonIds);
            if (reasons == null || reasons.Count != request.ReasonIds.Count)
                return ApiResponseFactory.BadRequest(_localizer["InvalidReasons"]);

            report.PlateNumber = request.PlateNumber.Trim().ToUpper();
            report.Description = request.Description;
            report.DriverId = driver.Id;

            report.Reasons.Clear();

            foreach (var reason in reasons)
            {
                report.Reasons.Add(new DriverReportReason
                {
                    Id = Guid.NewGuid(),
                    DriverReportId = report.Id,
                    ReportReasonId = reason.Id
                });
            }

            await _reportRepository.UpdateAsync(report);
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success(_localizer["ReportUpdated"]);
        }

        public async Task<ApiResponse> DeleteReportAsync(Guid passengerId, Guid reportId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);

            if (report == null)
                return ApiResponseFactory.NotFound(_localizer["ReportNotFound"]);

            if (report.PassengerId != passengerId)
                return ApiResponseFactory.Failure(_localizer["UnauthorizedDeleteReport"], 403);

            if (report.Status != ReportStatus.Pending)
                return ApiResponseFactory.Conflict(_localizer["ReportDeleteOnlyPending"]);

            await _reportRepository.DeleteAsync(report.Id);
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success(_localizer["ReportDeleted"]);
        }
    }
}
