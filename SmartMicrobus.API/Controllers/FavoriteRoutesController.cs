using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMicrobus.Core.DTO.Common;
using SmartMicrobus.Core.DTO.Route;
using SmartMicrobus.Core.Enums;
using SmartMicrobus.Core.ServiceContracts.Route;
using System.Security.Claims;

namespace SmartMicrobus.API.Controllers
{
    [Authorize(Roles = $"{nameof(UserRole.Passenger)}")]
    public class FavoriteRoutesController(IFavoriteRouteService favoriteRouteService) : CustomControllerBase
    {
        private readonly IFavoriteRouteService _favoriteRouteService = favoriteRouteService;
        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var passengerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _favoriteRouteService.GetFavoriteRoutesAsync(passengerId);
            if(!result.Success)
                return ToActionResult(result);

            var data = (result as ApiResponseWithData<List<FavoriteRouteResponse>>)?.Data;
            return Ok(data);
        }

        [HttpGet("{routeId}/is-favorite")]
        public async Task<IActionResult> IsFavorite(Guid routeId)
        {
            var passengerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _favoriteRouteService.IsFavoriteAsync(passengerId,routeId);           
                return ToActionResult(result);
        }

        [HttpPost("{routeId}")]
        public async Task<IActionResult> AddToFavorites(Guid routeId)
        {
            var passengerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _favoriteRouteService.AddToFavoritesAsync(passengerId, routeId);
            return ToActionResult(result);
        }

        [HttpDelete("{routeId}")]
        public async Task<IActionResult> RemoveFromFavorites(Guid routeId)
        {
            var passengerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _favoriteRouteService.RemoveFromFavoritesAsync(passengerId, routeId);
            return ToActionResult(result);
        }
    }
}
