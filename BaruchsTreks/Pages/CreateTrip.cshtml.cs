using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BaruchsTreks.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BaruchsTreks.Pages
{
    public class CreateTripModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public Guid TripId { get; set; }


        public CreateTripModel(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> OnGetAsync(string tripId)
        {

            if (!string.IsNullOrEmpty(tripId))
            {
                TripId = Guid.Parse(tripId);

                // Load existing trip for editing
                Trip = await _context.Trips.FirstOrDefaultAsync(t => t.id == TripId);
                if (Trip == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        [BindProperty]
        public Trip Trip { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var tripId = httpContextAccessor.HttpContext!.Request.Query["tripId"];
            Trip.id = Guid.TryParse(tripId, out var tripIdGuid) ? tripIdGuid : Guid.Empty;

            if (!ModelState.IsValid || Trip == null)
            {
                return Page();
            }

            if (Trip.id == Guid.Empty)
            {
                // Create new trip
                _context.Trips.Add(Trip);
            }
            else
            {
                // Update existing trip
                _context.Trips.Update(Trip);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
