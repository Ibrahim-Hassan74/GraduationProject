using Microsoft.Extensions.Options;
using SmartMicrobus.Core.Domain.Options;
using SmartMicrobus.Core.ServiceContracts.Common;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SmartMicrobus.Core.Services.Common
{
    public class CustomWhatsAppService: ICustomWhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWhatsAppSettings _settings;

        public CustomWhatsAppService(
            HttpClient httpClient,
            IOptions<CustomWhatsAppSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<bool> SendMessageAsync(string phoneNumber, string message)
        {
            var requestBody = new
            {
                number = phoneNumber,
                message = message
            };

            var json = JsonSerializer.Serialize(requestBody);

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                _settings.ApiUrl);

            request.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            request.Headers.Add("x-node-token", _settings.Token);

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}
