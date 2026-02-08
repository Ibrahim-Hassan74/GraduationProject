namespace SmartMicrobus.Core.DTO.WhatsApp
{
    public class WhatsAppTemplatePayload
    {
        public string messaging_product { get; set; } = "whatsapp";
        public string to { get; set; } = default!;
        public string type { get; set; } = "template";
        public WhatsAppTemplate template { get; set; } = default!;
    }
}
