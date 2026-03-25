using AutoMapper;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Report;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
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

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _reportRepository = unitOfWork.ReportRepository;
            _reasonRepository = unitOfWork.ReportReasonRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse> CreateReportAsync(Guid passengerId, CreateReportRequest request)
        {
           var validate= ValidationHelper.ModelValidation(request);
            if(!validate.Success)
                return validate;

            var driver = await _unitOfWork.MicrobusRepository.GetDriverAsync(request.PlateNumber);
            if (driver is null)
                return ApiResponseFactory.NotFound("Invalid plate number");

            var recentReports = await _reportRepository.HasRecentReportAsync(passengerId, request.PlateNumber);

            if (recentReports)
                return ApiResponseFactory.Conflict("You have already reported this driver recently.");

            var reasons = await _reasonRepository.GetByIdsAsync(request.ReasonIds);
            if (reasons == null || reasons.Count != request.ReasonIds.Count)
                return ApiResponseFactory.BadRequest("One or more reasons are invalid.");

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


            return ApiResponseFactory.Success("Report submitted successfully.");
        }

        public async Task<ApiResponse> GetReasonsAsync()
        {
            var reasons = await _reasonRepository.GetAllAsync();
            if (reasons == null || !reasons.Any())
                return ApiResponseFactory.NotFound("No report reasons found.");

            var list = reasons.Select(r => new ReportReasonResponse
            {
                Id = r.Id,
                Name = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar" ? r.NameAr : r.NameEn
            }).ToList();

            return ApiResponseFactory.Success("Reasons retrieved.", list);
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

            return ApiResponseFactory.Success("Reports retrieved successfully.", pagedResponse);
        }

        public async Task<ApiResponse> GetReportByIdAsync(Guid passengerId, Guid reportId)
        {
            var report = await _reportRepository.GetByIdWithReasonsAsync(reportId);

            if (report == null)
                return ApiResponseFactory.NotFound("Report not found.");
                       
            if (report.PassengerId != passengerId)
                return ApiResponseFactory.Failure("You are not authorized to access this report.", 403);

            var reportResponse = _mapper.Map<ReportResponseWithDetails>(report);

            return ApiResponseFactory.Success("Report retrieved successfully.", reportResponse);
        }

        public async Task<ApiResponse> UpdateReportAsync(Guid passengerId, Guid reportId, UpdateReportRequest request)
        {
            var validate = ValidationHelper.ModelValidation(request);
            if (!validate.Success)
                return validate;

            var report = await _reportRepository.GetByIdAsync(reportId,x=>x.Reasons);

            if (report == null)
                return ApiResponseFactory.NotFound("Report not found.");

           
            if (report.PassengerId != passengerId)
                return ApiResponseFactory.Failure("You are not authorized to update this report.", 403);

            if (report.Status != ReportStatus.Pending)
                return ApiResponseFactory.Conflict("You can only update reports that are in pending status.");
                       
            var driver = await _unitOfWork.MicrobusRepository.GetDriverAsync(request.PlateNumber);
            if (driver is null)
                return ApiResponseFactory.BadRequest("Invalid plate number.");

            var reasons = await _reasonRepository.GetByIdsAsync(request.ReasonIds);
            if (reasons == null || reasons.Count != request.ReasonIds.Count)
                return ApiResponseFactory.BadRequest("One or more reasons are invalid.");

            report.PlateNumber = request.PlateNumber;
            report.Description = request.Description;
            report.DriverId = driver?.Id;

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

            return ApiResponseFactory.Success("Report updated successfully.");
        }

        public async Task<ApiResponse> DeleteReportAsync(Guid passengerId, Guid reportId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);

            if (report == null)
                return ApiResponseFactory.NotFound("Report not found.");

            if (report.PassengerId != passengerId)
                return ApiResponseFactory.Failure("You are not authorized to delete this report.", 403);
                        
            if (report.Status != ReportStatus.Pending)
                return ApiResponseFactory.Conflict("You can only delete reports that are in pending status.");

            await _reportRepository.DeleteAsync(report.Id);
            await _unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("Report deleted successfully.");
        }
    }
}
