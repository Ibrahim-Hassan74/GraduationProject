using Microsoft.AspNetCore.Http;
using SmartMicrobus.Core.Domain.IdentityEntities;
using SmartMicrobus.Core.DTO.Common;

namespace SmartMicrobus.Core.ServiceContracts.Common
{
    public interface IImageService
    {
        Task<List<string>> AddImageAsync(IFormFileCollection files, string src);
        bool DeleteImageAsync(string src);
        Task<string> SaveUserAvatarAsync(IFormFile file, string folder, ImageCropDTO? crop);
    }
}
