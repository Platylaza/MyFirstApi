namespace MyFirstApi.Settings
{
    public class StoreSettings
    {
        public string StoreName { get; set; } = string.Empty;
        public string DefaultCurrency { get; set; } = "USD";
        public bool EnableDiscounts { get; set; }
    }
}