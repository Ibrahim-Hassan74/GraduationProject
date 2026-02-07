using Microsoft.Extensions.Options;
using SmartMicrobus.Core.Domain.Options;
using SmartMicrobus.Core.DTO.WhatsApp;
using SmartMicrobus.Core.ServiceContracts.Common;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SmartMicrobus.Core.Services.Common
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly WhatsAppSettings _settings;

        public WhatsAppService(
            HttpClient httpClient,
            IOptions<WhatsAppSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<bool> SendInvoiceMessageAsync(string phone, string code)
        {
            var payload = new WhatsAppTemplatePayload
            {
                to = phone,
                template = new WhatsAppTemplate
                {
                    name = _settings.TemplateName,
                    language = new WhatsAppLanguage { code = "ar" },
                    components = new List<WhatsAppComponent>
                    {
                        new WhatsAppComponent
                        {
                            type = "body",
                            parameters = new List<WhatsAppParameter>
                            {
                                new() { text = code }
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(payload);

            var request = new HttpRequestMessage(HttpMethod.Post, _settings.ApiUrl);
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.AccessToken);

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}
