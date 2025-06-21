using System.ComponentModel.DataAnnotations;

namespace Cloud_4.Models
{
    public class Venue
    {
        public int Id { get; set; }
        [StringLength(60, MinimumLength = 3)]
        public string? VenueName { get; set; }
        public string? Location { get; set; }
        public int Capacity { get; set; }

        public string? ImageURL { get; set; }
        public DateOnly? VenueDate { get; set; }
        public TimeOnly? VenueTime { get; set; }



    }
}
