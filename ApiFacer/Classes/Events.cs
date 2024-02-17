using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiFacer.Classes
{
    public class Events
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? path { get; set; }
    }
}
