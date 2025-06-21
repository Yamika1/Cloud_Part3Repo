using System.ComponentModel.DataAnnotations;

namespace Cloud_4.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        [DataType(DataType.Date)]
        public DateOnly BookingDate { get; set; }
        public string? BookingName { get; set; }

        public TimeOnly BookingTime { get; set; }

    }
}