using SmartMicrobus.Core.Domain.Entities;

namespace SmartMicrobus.Core.RepositoryContracts
{
    public interface IStaffRepository : IGenericRepository<Staff>
    {
        Task<Staff?> GetStaffByUserId(Guid userId);
        Task<(List<Staff> Staffs, int TotalCount)> GetPaginatedByStationAsync(Guid stationId, SmartMicrobus.Core.DTO.Staff.StaffQuery query);
    }
}
