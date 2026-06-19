using AutoMapper;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Microbus;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Microbus;

namespace SmartMicrobus.Core.Services.Microbus
{
    public class MicrobusService : IMicrobusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMicrobusRepository _microbusRepository;
        private readonly IMapper _mapper;

        public MicrobusService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _microbusRepository = _unitOfWork.MicrobusRepository;
        }

        public async Task<ApiResponse> GetPaginatedMicrobusesAsync(Guid stationId, MicrobusQuery query)
        {
            var (microbuses, totalCount) = await _microbusRepository.GetFilteredMicrobusesAsync(stationId, query);

            var response = _mapper.Map<List<MicrobusListResponse>>(microbuses);

            var result = new PagedResponse<MicrobusListResponse>
            {
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Items = response
            };

            return ApiResponseFactory.Success("Microbuses retrieved successfully.", result);
        }
        public async Task<ApiResponse> GetMicrobusByIdAsync(Guid microbusId, Guid stationId)
        {
            var microbus = await _microbusRepository.GetByIdWithDetailsAsync(microbusId, stationId);

            if (microbus is null)
                return ApiResponseFactory.NotFound("Microbus not found.");

            var response = _mapper.Map<MicrobusDetailsResponse>(microbus);

            return ApiResponseFactory.Success("Microbus retrieved successfully.", response);
        }
    }
}
