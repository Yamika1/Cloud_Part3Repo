using System.ComponentModel.DataAnnotations;

namespace Cloud_4.Models
{
    public class Event
    {
        public int EventId { get; set; }


        [Display(Name = "Event Date")]
        [DataType(DataType.Date)]
        public DateOnly EventDate { get; set; }
        public string? Description { get; set; }

        public string? EventName { get; set; }

        public TimeOnly EventTime { get; set; }
    }
}