namespace Cloud_4.Models.ViewModel
{
    public class SearchViewModel
    {
        public string? Query { get; set; }

        public List<Venue> Venues { get; set; } = new List<Venue>();
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Event> Events { get; set; } = new List<Event>();
    }
}

