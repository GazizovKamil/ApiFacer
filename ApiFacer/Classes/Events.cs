using System.ComponentModel.DataAnnotations;

namespace ApiFacer.Classes
{
    public class Events
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
