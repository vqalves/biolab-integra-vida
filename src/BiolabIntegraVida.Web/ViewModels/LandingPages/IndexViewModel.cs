namespace BiolabIntegraVida.Web.ViewModels.LandingPages
{
    public class IndexViewModel
    {
        public string GoogleMapsApiKey { get; init; }
        public string GoogleAnalyticsId { get; init; }
        
        public IndexViewModel(
            string googleMapsApiKey,
            string googleAnalyticsId)
        {
            GoogleMapsApiKey = googleMapsApiKey;
            GoogleAnalyticsId = googleAnalyticsId;
        }
    }
}