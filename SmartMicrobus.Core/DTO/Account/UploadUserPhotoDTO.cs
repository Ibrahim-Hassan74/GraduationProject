using Microsoft.AspNetCore.Http;
using SmartMicrobus.Core.DTO.Common;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class UploadUserPhotoDTO
    {
        [Required]
        public IFormFile File { get; set; }
        public ImageCropDTO? Crop { get; set; }
    }
}
