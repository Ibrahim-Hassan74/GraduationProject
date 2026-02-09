namespace SmartMicrobus.Core.DTO.WhatsApp
{
    public class WhatsAppParameter
    {
        public string type { get; set; } = "text";
        public string text { get; set; } = default!;
        public string coupon_code { get; set; } = null!;
    }
}
