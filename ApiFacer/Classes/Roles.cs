using System.ComponentModel.DataAnnotations;

namespace ApiFacer.Classes
{
    public class Roles
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
    }
}
