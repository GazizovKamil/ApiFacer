using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiFacer.Classes
{
    public class Images
    {
        [Key]
        public int Id { get; set; }
        public string? path { get; set; }
        public int authorId { get; set; }
        public int eventId { get; set; }
        [ForeignKey("eventId")]
        [JsonIgnore]
        public Events EventId { get; set; }
    }
}
