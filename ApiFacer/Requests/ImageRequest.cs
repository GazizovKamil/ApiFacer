namespace ApiFacer.Requests
{
    public class ImageRequest
    {
        public List<IFormFile> files { get; set; }
        public string sessionkey { get; set; }
    }
}
