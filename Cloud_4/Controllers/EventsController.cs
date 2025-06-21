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
    public class EventsController : Controller
    {
        private readonly Cloud_4Context _context;

        public EventsController(Cloud_4Context context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(string searchstring)
        {
            if (_context.Event == null)
            {
                return Problem("Entity set not found");
            }
            var events = from e in _context.Event
                         select e;

            if (!String.IsNullOrEmpty(searchstring))
            {
                events = events.Where(e => e.EventName!.ToUpper().Contains(searchstring.ToUpper()));
            }
            return View(await events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventDate,Description,EventName,EventTime")] Event @event)
        {
            if (ModelState.IsValid)
            {

                bool isDoubleBooked = _context.Event.Any(e => e.EventDate == @event.EventDate && e.EventTime == @event.EventTime);

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("", "An event is already booked at this date and time.");
                    return View(@event);

                }
                _context.Event.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventDate,Description,EventName,EventTime")] Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
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
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool events = await _context.Event.AnyAsync(gp => gp.EventId == id);

            if (events)
            {
                // retieve the game to redisplay Delete view with error
                var Events = await _context.Event.FindAsync(id);
                ModelState.AddModelError("", "Cannot delete this event, there are existing event records");

            }
            var bookingToDelete = await _context.Venue.FindAsync(id);
            _context.Venue.Remove(bookingToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventId == id);
        }
    }
}
