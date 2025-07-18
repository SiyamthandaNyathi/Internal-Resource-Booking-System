using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ResourceBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ResourceBookingSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Dashboard()
    {
        var today = DateTime.Today;
        var upcomingBookings = await _context.Bookings
            .Include(b => b.Resource)
            .Where(b => b.StartTime >= today)
            .OrderBy(b => b.StartTime)
            .ToListAsync();
        return View(upcomingBookings);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
