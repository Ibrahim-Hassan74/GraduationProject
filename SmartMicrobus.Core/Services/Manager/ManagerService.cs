using Microsoft.AspNetCore.Identity;
using SmartMicrobus.Core.Domain.Entities;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Driver;
using SmartMicrobus.Core.DTO.Microbus;
using SmartMicrobus.Core.Helper;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Core.ServiceContracts.Common;
using SmartMicrobus.Core.ServiceContracts.Manager;
using ClosedXML.Excel;

using AutoMapper;

namespace SmartMicrobus.Core.Services.Manager
{
    public class ManagerService(IUnitOfWork unitOfWork, IQrTokenService qrService, UserManager<ApplicationUser> userManager, IMapper mapper) : IManagerService
    {
        public async Task<ApiResponse> AddDriverAsync(DriverAddRequest driverAddRequest)
        {
            ApplicationUser user = new()
            {
                Id = Guid.NewGuid(),
                DisplayName = driverAddRequest.DriverName,
                UserName = driverAddRequest.PhoneNumber,
                PhoneNumber = driverAddRequest.PhoneNumber,
            };

            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
                return ApiResponseFactory.BadRequest("this user can't register for now");

            var driver = new Driver
            {
                Id = user.Id,
                LicenseNumber = driverAddRequest.LicenseNumber
            };

            var driverResponse = await unitOfWork.DriverRepository.AddAsync(driver);

            if (driverResponse == null)
                return ApiResponseFactory.BadRequest("this user can't register for now");

            await unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("driver added successfully", driverResponse.Id);
        }

        public async Task<ApiResponse> AddMicrobusAsync(MicrobusAddRequest microbusAddRequest)
        {
            var microbus = new Microbus
            {
                Id = Guid.NewGuid(),
                Color = microbusAddRequest.Color,
                Model = microbusAddRequest.Model,
                PassengerCount = microbusAddRequest.PassengerCount,
                PlateNumber = microbusAddRequest.PlateNumber,
                RouteId = microbusAddRequest.RouteId
            };

            var result = await unitOfWork.MicrobusRepository.AddAsync(microbus);

            if (result == null)
                return ApiResponseFactory.BadRequest("failed to add microbus");

            await unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("microbus added successfully", result.Id);
        }

        public async Task<ApiResponse> AssignDriverToMicrobusAsync(DriverAssignRequest driverAssignRequest)
        {
            var driver = await unitOfWork.DriverRepository.GetByIdAsync(driverAssignRequest.DriverId, d => d.ApplicationUser);
            var microbus = await unitOfWork.MicrobusRepository.GetByIdAsync(driverAssignRequest.MicrobusId);

            if (driver == null || microbus == null)
                return ApiResponseFactory.BadRequest("driver or microbus not found");

            var existingMicrobusForDriver = (await unitOfWork.MicrobusRepository.GetAllAsync())
                .FirstOrDefault(m => m.DriverId == driverAssignRequest.DriverId);
            
            if (existingMicrobusForDriver != null && existingMicrobusForDriver.Id != microbus.Id)
                return ApiResponseFactory.BadRequest("this driver is already assigned to another microbus");

            if (microbus.DriverId != null && microbus.DriverId != driver.Id)
                return ApiResponseFactory.BadRequest("this microbus is already assigned to another driver");

            var qrPayload = new MicrobusQrPayload
            {
                MicrobusId = microbus.Id,
                DriverId = driver.Id,
                RouteId = microbus.RouteId,
                DriverName = driver.ApplicationUser.DisplayName,
                PlateNumber = microbus.PlateNumber,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(1)
            };

            var qrCode = qrService.GenerateToken(qrPayload);

            if (qrCode == null)
                return ApiResponseFactory.BadRequest("failed to generate qr code");

            microbus.DriverId = driver.Id;
            microbus.QrCode = qrCode;

            await unitOfWork.MicrobusRepository.UpdateAsync(microbus);
            await unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("driver assigned to microbus successfully", qrCode);
        }

        public async Task<ApiResponse> GetManagerStationAsync(Guid managerId)
        {
            var manager = await unitOfWork.ManagerRepository.GetByIdAsync(managerId, m => m.Station);
            if (manager == null)
                return ApiResponseFactory.NotFound("Manager not found");

            return ApiResponseFactory.Success("Manager station retrieved successfully", manager.StationId);
        }
        public async Task<ApiResponse> GetStationDashboardAsync(Guid stationId)
        {
            var stats = await unitOfWork.StationRepository.GetDashboardStatsAsync(stationId);
            return ApiResponseFactory.Success("Dashboard stats retrieved successfully", stats);
        }

        public async Task<ApiResponseWithData<byte[]>> ExportStationDataExcelAsync(Guid managerId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var manager = await unitOfWork.ManagerRepository.GetByIdAsync(managerId, m => m.Station);
            if (manager == null)
                return ApiResponseFactory.NotFound<byte[]>("Manager not found");

            var trips = await unitOfWork.TripRepository.GetTripsByStationAndDateAsync(manager.StationId, startDate, endDate);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Trips");

            worksheet.Cell(1, 1).Value = "Trip ID";
            worksheet.Cell(1, 2).Value = "Driver Name";
            worksheet.Cell(1, 3).Value = "Microbus Plate";
            worksheet.Cell(1, 4).Value = "Route";
            worksheet.Cell(1, 5).Value = "Started At";
            worksheet.Cell(1, 6).Value = "Ended At";
            worksheet.Cell(1, 7).Value = "Distance (Km)";
            worksheet.Cell(1, 8).Value = "Total Amount";
            worksheet.Cell(1, 9).Value = "Status";

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;

            int row = 2;
            foreach (var trip in trips)
            {
                worksheet.Cell(row, 1).Value = trip.Id.ToString();
                worksheet.Cell(row, 2).Value = trip.Driver?.ApplicationUser?.DisplayName ?? "N/A";
                worksheet.Cell(row, 3).Value = trip.Microbus?.PlateNumber ?? "N/A";
                worksheet.Cell(row, 4).Value = (trip.Route?.FromEn != null && trip.Route?.ToEn != null) ? $"{trip.Route.FromEn} to {trip.Route.ToEn}" : "N/A";
                worksheet.Cell(row, 5).Value = trip.StartedAt.ToString("yyyy-MM-dd HH:mm");
                worksheet.Cell(row, 6).Value = trip.EndedAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A";
                worksheet.Cell(row, 7).Value = trip.DistanceKm;
                worksheet.Cell(row, 8).Value = trip.TotalAmount;
                worksheet.Cell(row, 9).Value = trip.Status.ToString();
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return ApiResponseFactory.Success("Excel generated successfully", content);
        }

 
        public async Task<ApiResponseWithData<byte[]>> ExportStationDriversExcelAsync(Guid managerId)
        {
            var manager = await unitOfWork.ManagerRepository.GetByIdAsync(managerId, m => m.Station);
            if (manager == null)
                return ApiResponseFactory.NotFound<byte[]>("Manager not found");

            var drivers = await unitOfWork.DriverRepository.GetDriversByStationAsync(manager.StationId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Drivers");

            worksheet.Cell(1, 1).Value = "Driver ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Phone Number";
            worksheet.Cell(1, 4).Value = "License Number";
            worksheet.Cell(1, 5).Value = "Microbus Plate";
            worksheet.Cell(1, 6).Value = "Assigned Route";

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;

            int row = 2;
            foreach (var driver in drivers)
            {
                worksheet.Cell(row, 1).Value = driver.Id.ToString();
                worksheet.Cell(row, 2).Value = driver.ApplicationUser?.DisplayName ?? "N/A";
                worksheet.Cell(row, 3).Value = driver.ApplicationUser?.PhoneNumber ?? "N/A";
                worksheet.Cell(row, 4).Value = driver.LicenseNumber ?? "N/A";
                worksheet.Cell(row, 5).Value = driver.Microbus?.PlateNumber ?? "N/A";
                worksheet.Cell(row, 6).Value = (driver.Microbus?.Route?.FromEn != null && driver.Microbus?.Route?.ToEn != null) ? $"{driver.Microbus.Route.FromEn} to {driver.Microbus.Route.ToEn}" : "N/A";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return ApiResponseFactory.Success("Excel generated successfully", content);
        }

        public async Task<ApiResponseWithData<byte[]>> ExportStationRoutesExcelAsync(Guid managerId)
        {
            var manager = await unitOfWork.ManagerRepository.GetByIdAsync(managerId, m => m.Station);
            if (manager == null)
                return ApiResponseFactory.NotFound<byte[]>("Manager not found");

            var routes = await unitOfWork.RouteRepository.GetRoutesByFromAsync(manager.StationId);
            var activeBuses = await unitOfWork.MicrobusRepository.GetActiveMicrobusesByStationAsync(manager.StationId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Drivers");

            worksheet.Cell(1, 1).Value = "Route ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Price";
            worksheet.Cell(1, 4).Value = "Distance";
            worksheet.Cell(1, 5).Value = "Active Buses";

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;

            int row = 2;
            foreach (var route in routes)
            {
                worksheet.Cell(row, 1).Value = route.Id.ToString();
                worksheet.Cell(row, 2).Value = route.FromEn + " to " + route.ToEn;
                worksheet.Cell(row, 3).Value = route.Price;
                worksheet.Cell(row, 4).Value = route.DistanceKm;
                worksheet.Cell(row, 5).Value = activeBuses.Where(m => m.RouteId == route.Id).Count();
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return ApiResponseFactory.Success("Excel generated successfully", content);
        }

        public async Task<ApiResponseWithData<byte[]>> ExportMicrobusesExcelAsync(Guid managerId)
        {
            var manager = await unitOfWork.ManagerRepository.GetByIdAsync(managerId, m => m.Station);
            if (manager == null)
                return ApiResponseFactory.NotFound<byte[]>("Manager not found");

            var microbuses = await unitOfWork.MicrobusRepository.GetAllStationMicrobusesAsync(manager.StationId);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Drivers");

            worksheet.Cell(1, 1).Value = "Microbus ID";
            worksheet.Cell(1, 2).Value = "Model";
            worksheet.Cell(1, 3).Value = "Capacity";
            worksheet.Cell(1, 4).Value = "Color";
            worksheet.Cell(1, 5).Value = "Status";

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;

            int row = 2;
            foreach (var microbus in microbuses)
            {
                worksheet.Cell(row, 1).Value = microbus.Id.ToString();
                worksheet.Cell(row, 2).Value = microbus.Model;
                worksheet.Cell(row, 3).Value = microbus.PassengerCount;
                worksheet.Cell(row, 4).Value = microbus.Color;
                worksheet.Cell(row, 5).Value = microbus.IsActive ? "Active" : "Inactive";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return ApiResponseFactory.Success("Excel generated successfully", content);
        }

        public async Task<ApiResponse> GetPaginatedStationMicrobusesAsync(MicrobusQuery query, Guid stationId)
        {
            var (microbuses, totalCount) = await unitOfWork.MicrobusRepository.GetPaginatedByStationAsync(stationId, query);

            var mappedMicrobuses = mapper.Map<List<MicrobusResponse>>(microbuses);
            var result = new Pagination<List<MicrobusResponse>>(query.PageNumber, query.PageSize, totalCount, mappedMicrobuses);

            return ApiResponseFactory.Success("Paginated microbuses retrieved successfully.", result);
        }

        public async Task<ApiResponse> GetPaginatedStationDriversAsync(DriverQuery query, Guid stationId)
        {
            var (drivers, totalCount) = await unitOfWork.DriverRepository.GetPaginatedByStationAsync(stationId, query);

            var mappedDrivers = mapper.Map<List<DriverResponse>>(drivers);
            var result = new Pagination<List<DriverResponse>>(query.PageNumber, query.PageSize, totalCount, mappedDrivers);

            return ApiResponseFactory.Success("Paginated drivers retrieved successfully.", result);
        }

        public async Task<ApiResponse> AddStaffAsync(SmartMicrobus.Core.DTO.Staff.AddStaffDTO dto, Guid stationId)
        {
            var existingUser = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
            if (existingUser != null)
                return ApiResponseFactory.BadRequest("A user with this phone number already exists.");

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                DisplayName = dto.Name,
                UserName = dto.PhoneNumber,
                PhoneNumber = dto.PhoneNumber,
                PhoneNumberConfirmed = true
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return ApiResponseFactory.BadRequest($"Failed to create user: {errors}");
            }

            // Ensure the Staff role is assigned
            await userManager.AddToRoleAsync(user, SmartMicrobus.Core.Enums.UserRole.Staff.ToString());

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                StationId = stationId,
                IsActive = true
            };

            await unitOfWork.StaffRepository.AddAsync(staff);
            await unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("Staff added successfully.");
        }

        public async Task<ApiResponse> UpdateStaffAsync(Guid staffId, SmartMicrobus.Core.DTO.Staff.UpdateStaffDTO dto, Guid stationId)
        {
            var staff = await unitOfWork.StaffRepository.GetByIdAsync(staffId, s => s.User);
            if (staff == null || staff.StationId != stationId || staff.User.IsDeleted)
                return ApiResponseFactory.NotFound("Staff not found.");

            var existingUser = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber && u.Id != staff.UserId);
            if (existingUser != null)
                return ApiResponseFactory.BadRequest("A user with this phone number already exists.");

            staff.User.DisplayName = dto.Name;
            staff.User.PhoneNumber = dto.PhoneNumber;
            staff.User.UserName = dto.PhoneNumber;
            staff.IsActive = dto.IsActive;

            await userManager.UpdateAsync(staff.User);
            await unitOfWork.StaffRepository.UpdateAsync(staff);
            await unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("Staff updated successfully.");
        }

        public async Task<ApiResponse> DeleteStaffAsync(Guid staffId, Guid stationId)
        {
            var staff = await unitOfWork.StaffRepository.GetByIdAsync(staffId, s => s.User);
            if (staff == null || staff.StationId != stationId || staff.User.IsDeleted)
                return ApiResponseFactory.NotFound("Staff not found.");

            staff.IsActive = false;

            // Soft delete user
            var originalPhone = staff.User.PhoneNumber;
            staff.User.IsDeleted = true;
            staff.User.PhoneNumber = $"DELETED_{staff.User.Id}_{originalPhone}";
            staff.User.UserName = staff.User.PhoneNumber;
            staff.User.PhoneNumberConfirmed = false;

            await userManager.UpdateAsync(staff.User);
            await unitOfWork.StaffRepository.UpdateAsync(staff);
            await unitOfWork.CompleteAsync();

            return ApiResponseFactory.Success("Staff deleted successfully.");
        }

        public async Task<ApiResponse> GetPaginatedStationStaffAsync(SmartMicrobus.Core.DTO.Staff.StaffQuery query, Guid stationId)
        {
            var (staffs, totalCount) = await unitOfWork.StaffRepository.GetPaginatedByStationAsync(stationId, query);

            var mappedStaff = staffs.Select(staff => new SmartMicrobus.Core.DTO.Staff.StaffResponseDTO
            {
                Id = staff.Id,
                UserId = staff.UserId,
                Name = staff.User.DisplayName,
                PhoneNumber = staff.User.PhoneNumber ?? "",
                IsActive = staff.IsActive,
                HasPassword = true
            }).ToList();

            var result = new Pagination<List<SmartMicrobus.Core.DTO.Staff.StaffResponseDTO>>(query.PageNumber, query.PageSize, totalCount, mappedStaff);
            return ApiResponseFactory.Success("Paginated staff retrieved successfully.", result);
        }
    }
}
