namespace SmartMicrobus.Core.DTO.WhatsApp
{
    public class WhatsAppTemplate
    {
        public string name { get; set; } = default!;
        public WhatsAppLanguage language { get; set; } = default!;
        public List<WhatsAppComponent>? components { get; set; }
    }
}
