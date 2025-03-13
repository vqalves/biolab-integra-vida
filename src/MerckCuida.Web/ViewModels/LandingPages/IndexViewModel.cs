namespace MerckCuida.Web.ViewModels.LandingPages
{
    public class IndexViewModel
    {
        public string GoogleMapsApiKey { get; init; }

        public IndexViewModel(string googleMapsApiKey)
        {
            GoogleMapsApiKey = googleMapsApiKey;
        }
    }
}
