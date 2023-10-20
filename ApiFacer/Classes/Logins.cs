using System.ComponentModel.DataAnnotations;

namespace ApiFacer.Classes
{
    public class Logins
    {
        [Key]
        public int Id { get; set; }
        public string sessionkey { get; set; }
        public string ipAdress { get; set; }
        public int userId { get; set; }
    }
}
