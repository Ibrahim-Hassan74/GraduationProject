namespace SmartMicrobus.Core.DTO.WhatsApp
{
    public class WhatsAppComponent
    {
        public string type { get; set; } = default!;
        public string? sub_type { get; set; }
        public string? index { get; set; }
        public List<WhatsAppParameter>? parameters { get; set; }
    }
}
