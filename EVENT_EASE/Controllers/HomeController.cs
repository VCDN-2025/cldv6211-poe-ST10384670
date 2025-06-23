using System.Diagnostics;
using EVENT_EASE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EVENT_EASE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Cldvdb1Context _context; // Inject your DB context

        public HomeController(ILogger<HomeController> logger, Cldvdb1Context context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bookingCount = await _context.Bookings.CountAsync();
            var venueCount = await _context.Venues.CountAsync();
            var eventCount = await _context.Events.CountAsync();

            ViewBag.BookingCount = bookingCount;
            ViewBag.VenueCount = venueCount;
            ViewBag.EventCount = eventCount;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
