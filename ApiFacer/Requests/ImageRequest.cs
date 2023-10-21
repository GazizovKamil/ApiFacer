namespace ApiFacer.Requests
{
    public class ImageRequest
    {
        public IFormFileCollection files { get; set; }
        public string sessionkey { get; set; }
    }
}
