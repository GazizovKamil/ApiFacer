using System;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiFacer.Classes
{
    public class UserImages
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ImageId")]
        public int ImageId { get; set; }
        public Images Image { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public People People { get; set; }
    }
}
