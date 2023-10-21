namespace ApiFacer.Requests
{
    public class CreateEvent
    {
        public string Name { get; set; }
        public string sessionkey { get; set; }
        public int? parentEventId { get; set; }
    }
}
