namespace ApiFacer.Requests
{
    public class AddUser
    {
        public string? login { get; set; }
        public string? password { get; set; }
        public string? surname { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string sessionkey { get; set; }
    }
}
