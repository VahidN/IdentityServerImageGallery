namespace ImageGallery.MvcClient.ViewModels
{
    public class OrderFrameViewModel
    {
        public string Address { get; } = string.Empty;

        public OrderFrameViewModel(string address)
        {
            Address = address;
        }
    }
}
