using System.ComponentModel.DataAnnotations;

namespace ApiFacer.Classes
{
    public class People
    {
        [Key]
        public int Id { get; set; }
        public string? surname { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
    }
}
