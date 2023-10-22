using System.ComponentModel.DataAnnotations;

namespace ApiFacer.Classes
{
    public class UserImages
    {
        [Key]
        public int Id { get; set; }
        public int ImageId { get; set; }
        public int UserId { get; set; }
    }
}
