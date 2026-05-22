using Microsoft.AspNetCore.Http;
using SmartMicrobus.Core.DTO.Common;
using System.ComponentModel.DataAnnotations;

namespace SmartMicrobus.Core.DTO.Account
{
    public class UploadUserPhotoDTO
    {
        [Display(Name = "File", ResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        [Required(ErrorMessageResourceName = "RequiredFile",
          ErrorMessageResourceType = typeof(Resources.DTO.Account.AuthValidationMessages))]
        public IFormFile File { get; set; }
        public ImageCropDTO? Crop { get; set; }
    }
}
