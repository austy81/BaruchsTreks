using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BaruchsTreks.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using BaruchsTreks.Enums;

namespace BaruchsTreks.Pages
{
    [Authorize]
    public class CreateTripModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public Guid TripId { get; set; }

        [BindProperty]
        public Trip trip { get; set; } = new Trip();

        [BindProperty]
        public string Plo { get; set; } = string.Empty;
        [BindProperty]
        public string Pla { get; set; } = string.Empty;
        [BindProperty]
        public string Hlo { get; set; } = string.Empty;
        [BindProperty]
        public string Hla { get; set; } = string.Empty;

        [BindProperty]
        public List<KeyValuePair<string, string>> UiaaGradeList => GetEnumDisplayList<UiaaGradeEnum>();

        [BindProperty]
        public List<KeyValuePair<string, string>> AlpineGradeList => GetEnumDisplayList<AlpineGradeEnum>();

        [BindProperty]
        public List<KeyValuePair<string, string>> TripClassList => GetEnumDisplayList<TripClassEnum>();

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
                trip = await _context.Trips.FirstOrDefaultAsync(t => t.id == TripId) ?? new Trip();
                if (trip == null || trip.id == Guid.Empty)
                {
                    return NotFound();
                }
                else
                {
                    Hlo = trip.HighPoint.Longtitude.ToString(CultureInfo.InvariantCulture);
                    Hla = trip.HighPoint.Latitude.ToString(CultureInfo.InvariantCulture);
                    Plo = trip.Parking.Longtitude.ToString(CultureInfo.InvariantCulture);
                    Pla = trip.Parking.Latitude.ToString(CultureInfo.InvariantCulture);
                }
            }

            return Page();
        }




        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var tripId = httpContextAccessor.HttpContext!.Request.Query["tripId"];
            trip.id = Guid.TryParse(tripId, out var tripIdGuid) ? tripIdGuid : Guid.Empty;

            if (!ModelState.IsValid || trip == null)
            {
                return Page();
            }

            trip.HighPoint.Longtitude =  double.Parse(Hlo.Replace(',', '.'), CultureInfo.InvariantCulture);
            trip.HighPoint.Latitude = double.Parse(Hla.Replace(',', '.'), CultureInfo.InvariantCulture);
            trip.Parking.Longtitude= double.Parse(Plo.Replace(',', '.'), CultureInfo.InvariantCulture);
            trip.Parking.Latitude = double.Parse(Pla.Replace(',', '.'), CultureInfo.InvariantCulture);

            if (trip.id == Guid.Empty)
            {
                // Create new trip
                _context.Trips.Add(trip);
            }
            else
            {
                // Update existing trip
                _context.Trips.Update(trip);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        private List<KeyValuePair<string, string>> GetEnumDisplayList<TEnum>() where TEnum : Enum
        {
            var result = new List<KeyValuePair<string, string>>();
            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                result.Add(new KeyValuePair<string, string>(enumValue.ToString(), enumValue.DisplayNameForEnum()));
            }
            return result;
        }

    }
}
