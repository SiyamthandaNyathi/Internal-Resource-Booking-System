using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Models;

namespace ResourceBookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string search)
        {
            var bookings = _context.Bookings.Include(b => b.Resource).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                bookings = bookings.Where(b =>
                    b.Resource.Name.Contains(search) ||
                    b.BookedBy.Contains(search) ||
                    b.Purpose.Contains(search));
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

            var booking = await _context.Bookings
                .Include(b => b.Resource)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create(int? ResourceId)
        {
            if (ResourceId.HasValue)
            {
                ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", ResourceId.Value);
            }
            else
            {
                ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name");
            }
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ResourceId,StartTime,EndTime,BookedBy,Purpose")] Booking booking)
        {
            // Debug: Log incoming booking data
            Console.WriteLine($"[DEBUG] Booking POST: ResourceId={booking.ResourceId}, StartTime={booking.StartTime}, EndTime={booking.EndTime}, BookedBy={booking.BookedBy}, Purpose={booking.Purpose}");

            // Server-side validation for required fields and logical constraints
            if (booking.StartTime == default)
            {
                ModelState.AddModelError("StartTime", "Start time is required.");
            }
            if (booking.EndTime == default)
            {
                ModelState.AddModelError("EndTime", "End time is required.");
            }
            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time.");
            }
            if (string.IsNullOrWhiteSpace(booking.Purpose))
            {
                ModelState.AddModelError("Purpose", "Purpose is required.");
            }

            // Booking conflict logic: check if any existing booking for the same resource overlaps with the requested time slot
            var conflict = await _context.Bookings
                .Where(b => b.ResourceId == booking.ResourceId &&
                            b.EndTime > booking.StartTime &&
                            b.StartTime < booking.EndTime)
                .AnyAsync();
            if (conflict)
            {
                ModelState.AddModelError(string.Empty, "This resource is already booked during the requested time. Please choose another slot or resource, or adjust your times.");
            }

            // Debug: Log ModelState errors if any
            if (!ModelState.IsValid)
            {
                Console.WriteLine("[DEBUG] ModelState is invalid. Errors:");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"[DEBUG] Field: {key}, Error: {error.ErrorMessage}");
                    }
                }
            }
            else
            {
                Console.WriteLine("[DEBUG] ModelState is valid. Proceeding to save booking.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Booking created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", booking.ResourceId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", booking.ResourceId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ResourceId,StartTime,EndTime,BookedBy,Purpose")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            // Server-side validation for required fields and logical constraints
            if (booking.StartTime == default)
            {
                ModelState.AddModelError("StartTime", "Start time is required.");
            }
            if (booking.EndTime == default)
            {
                ModelState.AddModelError("EndTime", "End time is required.");
            }
            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time.");
            }
            if (string.IsNullOrWhiteSpace(booking.Purpose))
            {
                ModelState.AddModelError("Purpose", "Purpose is required.");
            }

            // Booking conflict logic: exclude the current booking from the check
            // This ensures that when editing, the booking does not conflict with other bookings for the same resource
            var conflict = await _context.Bookings
                .Where(b => b.ResourceId == booking.ResourceId &&
                            b.Id != booking.Id &&
                            b.EndTime > booking.StartTime &&
                            b.StartTime < booking.EndTime)
                .AnyAsync();
            if (conflict)
            {
                // Add a user-friendly error message if a conflict is detected
                ModelState.AddModelError(string.Empty, "This resource is already booked during the requested time. Please choose another slot or resource, or adjust your times.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency issues gracefully
                    if (!BookingExists(booking.Id))
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
            ViewData["ResourceId"] = new SelectList(_context.Resources, "Id", "Name", booking.ResourceId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Resource)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                TempData["SuccessMessage"] = "Booking deleted successfully!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
