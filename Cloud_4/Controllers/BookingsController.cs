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
    public class BookingsController : Controller
    {
        private readonly Cloud_4Context _context;

        public BookingsController(Cloud_4Context context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string searchstring)
        {
            if (_context.Booking == null)
            {
                return Problem("Entity set not found");
            }
            var bookings = from b in _context.Booking
                           select b;

            if (!String.IsNullOrEmpty(searchstring))
            {
                bookings = bookings.Where(b => b.BookingName!.ToUpper().Contains(searchstring.ToUpper()));
            }
            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,BookingDate,BookingName,BookingTime")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                bool isDoubleBooked = _context.Booking.Any(b => b.BookingDate == booking.BookingDate && b.BookingTime == booking.BookingTime);

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("", "An event is already booked at this date, time and venue.");
                    return View(booking);
                }
                _context.Booking.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,BookingDate,BookingName,BookingTime")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool bookings = await _context.Booking.AnyAsync(gp => gp.BookingId == id);

            if (bookings)
            {
                // retieve the game to redisplay Delete view with error
                var booking = await _context.Booking.FindAsync(id);
                ModelState.AddModelError("", "Cannot delete this booking, there are existing booking records");
                return View(booking);

            }
            var bookingToDelete = await _context.Event.FindAsync(id);
            _context.Event.Remove(bookingToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }
        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }
    }
}
