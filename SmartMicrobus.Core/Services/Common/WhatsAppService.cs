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

        public async Task<bool> SendOTPAsync(string phone, string code, string templateName)
        {
            var payload = new WhatsAppTemplatePayload
            {
                messaging_product = "whatsapp",
                to = phone,
                type = "template",
                template = new WhatsAppTemplate
                {
                    name = templateName,
                    language = new WhatsAppLanguage { code = "ar_eg" },
                    components = new List<WhatsAppComponent>
                    {
                        new WhatsAppComponent
                        {
                            type = "body",
                            parameters = new List<WhatsAppParameter>
                            {
                                new WhatsAppParameter
                                {
                                    type = "text",
                                    text = code
                                }
                            }
                        },

                        new WhatsAppComponent
                        {
                            type = "button",
                            sub_type = "copy_code",
                            index = "0",
                            parameters = new List<WhatsAppParameter>
                            {
                                new WhatsAppParameter
                                {
                                    type = "coupon_code",
                                    coupon_code = code
                                }
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
