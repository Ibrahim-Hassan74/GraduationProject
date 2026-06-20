using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.DTO.Staff;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IStaffRepository : IGenericRepository<Staff>
    {
        Task<Staff?> GetStaffByUserId(Guid userId);
        Task<(List<Staff> Staffs, int TotalCount)> GetPaginatedByStationAsync(Guid stationId, StaffQuery query);
    }
}
