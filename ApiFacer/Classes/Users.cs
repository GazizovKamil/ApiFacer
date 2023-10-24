using System.ComponentModel.DataAnnotations;

namespace ApiFacer.Classes
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        public string? login { get; set; }
        public string? password { get; set; }
        public string? surname { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public int id_role { get; set; }
    }
}
