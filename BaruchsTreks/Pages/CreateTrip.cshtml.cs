using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BaruchsTreks.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BaruchsTreks.Pages
{
    public class CreateTripModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public Guid TripId { get; set; }

        [BindProperty]
        public Trip Trip { get; set; } = new Trip();

        [BindProperty]
        public string Plo { get; set; }
        [BindProperty]
        public string Pla { get; set; }
        [BindProperty]
        public string Hlo { get; set; }
        [BindProperty]
        public string Hla { get; set; }

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
                else
                {
                    Hlo = Trip.HighPoint.Longtitude.ToString(CultureInfo.InvariantCulture);
                    Hla = Trip.HighPoint.Latitude.ToString(CultureInfo.InvariantCulture);
                    Plo = Trip.Parking.Longtitude.ToString(CultureInfo.InvariantCulture);
                    Pla = Trip.Parking.Latitude.ToString(CultureInfo.InvariantCulture);
                }
            }

            return Page();
        }




        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var tripId = httpContextAccessor.HttpContext!.Request.Query["tripId"];
            Trip.id = Guid.TryParse(tripId, out var tripIdGuid) ? tripIdGuid : Guid.Empty;

            if (!ModelState.IsValid || Trip == null)
            {
                return Page();
            }

            Trip.HighPoint.Longtitude =  double.Parse(Hlo.Replace(',', '.'), CultureInfo.InvariantCulture);
            Trip.HighPoint.Latitude = double.Parse(Hla.Replace(',', '.'), CultureInfo.InvariantCulture);
            Trip.Parking.Longtitude= double.Parse(Plo.Replace(',', '.'), CultureInfo.InvariantCulture);
            Trip.Parking.Latitude = double.Parse(Pla.Replace(',', '.'), CultureInfo.InvariantCulture);

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
