using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cloud_4.Data;
using Cloud_4.Models;

namespace Cloud_4.Controllers
{
    public class VenuesController : Controller
    {
        private readonly Cloud_4Context _context;

        public VenuesController(Cloud_4Context context)
        {
            _context = context;
        }

        // GET: Venues
        public async Task<IActionResult> Index(string searchstring)
        {
            if (_context.Venue == null)
            {
                return Problem("Entity set not found");
            }
            var venues = from v in _context.Venue
                         select v;

            if (!String.IsNullOrEmpty(searchstring))
            {
                venues = venues.Where(e => e.VenueName!.ToUpper().Contains(searchstring.ToUpper()));
            }
            return View(await venues.ToListAsync());
        }
        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VenueName,Location,Capacity,ImageURL,VenueDate,VenueTime")] Venue venue)
        {
            if (ModelState.IsValid)
            {

                bool isDoubleBooked = _context.Venue.Any(v => v.VenueDate == venue.VenueDate && v.VenueTime == venue.VenueTime);

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("", "An event is already booked at this date and time.");
                    return View(venue);

                }
                _context.Venue.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VenueName,Location,Capacity,ImageURL,VenueDate,VenueTime")] Venue venue)
        {
            if (id != venue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.Id == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool venues = await _context.Venue.AnyAsync(gp => gp.Id == id);

            if (venues)
            {
                // retieve the game to redisplay Delete view with error
                var Venue = await _context.Venue.FindAsync(id);
                ModelState.AddModelError("", "Cannot delete this venue, there are existing venue records");
                return View(Venue);

            }
            var bookingToDelete = await _context.Booking.FindAsync(id);
            _context.Booking.Remove(bookingToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }
        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.Id == id);
        }
    }
}
