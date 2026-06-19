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

namespace SmartMicrobus.Core.Services.Manager
{
    public class ManagerService(IUnitOfWork unitOfWork, IQrTokenService qrService, UserManager<ApplicationUser> userManager) : IManagerService
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
    }
}
