
namespace SmartMicrobus.Core.ServiceContracts.Common
{
    public interface ICustomWhatsAppService
    {
        Task<bool> SendMessageAsync(string phoneNumber, string message);
    }
}
