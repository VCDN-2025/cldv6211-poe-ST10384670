using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EVENT_EASE.Models;
using EVENT_EASE.Services; // ✅ Include this for BlobStorageService
using Microsoft.AspNetCore.Http;
using System.IO;

namespace EVENT_EASE.Controllers
{
    public class EventController : Controller
    {
        private readonly Cldvdb1Context _context;
        private readonly BlobStorageService _blobStorageService;

        public EventController(Cldvdb1Context context, BlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }

        // GET: Event
        public async Task<IActionResult> Index()
        {
            var cldvdb1Context = _context.Events.Include(e => e.Venue)
                .Include(e => e.EventType);
            return View(await cldvdb1Context.ToListAsync());
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null)
                return NotFound();

            return View(@event);
        }

        // GET: Event/Create
        public IActionResult Create()
        {
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name");
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name");
            return View();
        }

        // POST: Event/Create (with image upload)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,Title,Description,Date,Time,VenueId,EventTypeId")] Event @event, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var container = await _blobStorageService.GetContainerAsync();
                    var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var blobClient = container.GetBlobClient(fileName);

                    using (var stream = ImageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }

                    @event.ImageUrl = blobClient.Uri.ToString();
                }

                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", @event.VenueId);
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name", @event.EventTypeId);
            
            return View(@event);
        }

        // GET: Event/Edit/5
        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
                return NotFound();

            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", @event.VenueId);
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name", @event.EventTypeId); // ✅ Add this

            return View(@event);
        }


        // POST: Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Title,Description,Date,Time,VenueId,EventTypeId")] Event @event) // ✅ Include EventTypeId
        {
            if (id != @event.EventId)
                return NotFound();

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
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", @event.VenueId);
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name", @event.EventTypeId); // ✅ Add this

            return View(@event);
        }


        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null)
                return NotFound();

            return View(@event);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            try
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                // Log the exception if you want

                TempData["ErrorMessage"] = "Unable to delete event. It might be referenced elsewhere or has constraints.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }


        // ✅ Search Action
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                var allEvents = await _context.Events.Include(e => e.Venue).ToListAsync();
                return View("Index", allEvents);
            }

            var pattern = $"%{query}%";

            var filteredEvents = await _context.Events
                .Include(e => e.Venue)
                .Where(e =>
                    EF.Functions.Like(e.Title, pattern) ||
                    (e.Description != null && EF.Functions.Like(e.Description, pattern))
                )
                .ToListAsync();

            return View("Index", filteredEvents);
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
