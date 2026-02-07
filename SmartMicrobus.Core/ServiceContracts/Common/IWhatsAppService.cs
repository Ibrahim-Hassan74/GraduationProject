namespace SmartMicrobus.Core.ServiceContracts.Common
{
    public interface IWhatsAppService
    {
        Task<bool> SendInvoiceMessageAsync(string phone, string message);
    }
}
