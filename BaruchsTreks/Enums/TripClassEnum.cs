using System.ComponentModel.DataAnnotations;

namespace BaruchsTreks.Enums
{
    public enum TripClassEnum
    {
        [Display(Name = "none")]
        none,

        [Display(Name = "VHT")]
        Mountaineering,

        [Display(Name = "Skialpy")]
        Skialp,

        [Display(Name = "Sjezdovka")]
        Slope,

        [Display(Name = "Lezení")]
        Climb,

        [Display(Name = "Běh")]
        Run,

        [Display(Name = "Via ferrata")]
        Ferratta,

        [Display(Name = "Trek")]
        Trail
    }
}
