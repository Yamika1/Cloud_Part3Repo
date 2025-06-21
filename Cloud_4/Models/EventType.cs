using System.ComponentModel.DataAnnotations;

namespace Cloud_4.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeId { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Required]
        public string? Name { get; set; }
        [Required]
        public bool Availability { get; set; }
        public string? Event_Type { get; set; }

    }
}