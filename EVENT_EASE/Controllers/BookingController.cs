using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EVENT_EASE.Models;

namespace EVENT_EASE.Controllers
{
    public class BookingController : Controller
    {
        private readonly Cldvdb1Context _context;

        public BookingController(Cldvdb1Context context)
        {
            _context = context;
        }

        // GET: Booking
        public async Task<IActionResult> Index()
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Event.EventType)
                .Include(b => b.Venue);
            return View(await bookings.ToListAsync());
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // GET: Booking/Create
        public IActionResult Create()
        {
            ViewBag.EventId = new SelectList(_context.Events, "EventId", "Title");
            ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "Name");
            return View();
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,ClientName,ClientEmail,NumberOfGuests,EventId,VenueId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.BookingDate = DateTime.Now;
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.EventId = new SelectList(_context.Events, "EventId", "Title", booking.EventId);
            ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);
            return View(booking);
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            ViewBag.EventId = new SelectList(_context.Events, "EventId", "Title", booking.EventId);
            ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);
            return View(booking);
        }

        // POST: Booking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,ClientName,ClientEmail,NumberOfGuests,EventId,VenueId")] Booking booking)
        {
            if (id != booking.BookingId)
                return NotFound();

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
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.EventId = new SelectList(_context.Events, "EventId", "Title", booking.EventId);
            ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);
            return View(booking);
        }

        // GET: Booking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
                _context.Bookings.Remove(booking);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }

        //All Bookings view
        public async Task<IActionResult> BookingSummary(string searchBookingId, string searchEventType, string searchEventName, bool? searchAvailability, DateTime? startDate, DateTime? endDate)
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchBookingId) && int.TryParse(searchBookingId, out int bookingId))
            {
                bookings = bookings.Where(b => b.BookingId == bookingId);
            }

            if (!string.IsNullOrEmpty(searchEventType))
            {
                bookings = bookings.Where(b => b.Event.EventType.Name.Contains(searchEventType));
            }

            if (!string.IsNullOrEmpty(searchEventName))
            {
                bookings = bookings.Where(b => b.Event.Title.Contains(searchEventName));
            }

            if (searchAvailability.HasValue)
            {
                bookings = bookings.Where(b => b.Venue.IsAvailable == searchAvailability.Value);
            }

            if (startDate.HasValue)
            {
                bookings = bookings.Where(b => b.Event.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                bookings = bookings.Where(b => b.Event.Date <= endDate.Value);
            }

            var filteredResults = await bookings.ToListAsync();
            return View(filteredResults);
        }

    }
}
