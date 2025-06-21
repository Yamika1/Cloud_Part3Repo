using Cloud_4.Data;
using Cloud_4.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cloud_4.Controllers
{
    public class SearchController : Controller
    {
        private readonly Cloud_4Context _context;

        public SearchController(Cloud_4Context context)
        {
            _context = context;
        }
        public IActionResult Index(string query)
        {
            var viewModel = new SearchViewModel
            {
                Query = query
            };

            if (!string.IsNullOrWhiteSpace(query))
            {
                viewModel.Bookings = _context.Booking
                    .Where(c => c.BookingName.Contains(query))
                    .ToList();

                viewModel.Venues = _context.Venue
                    .Where(o => o.VenueName.Contains(query))
                    .ToList();

                viewModel.Events = _context.Event
                    .Where(p => p.EventName.Contains(query))
                    .ToList();
            }

            return View(viewModel);
        }
    }
}