namespace SmartMicrobus.Core.ServiceContracts.Common
{
    public interface IWhatsAppService
    {
        Task<bool> SendOTPAsync(string phone, string code, string templateName);
    }
}
