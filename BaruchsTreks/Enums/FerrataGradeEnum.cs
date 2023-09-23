using System.ComponentModel.DataAnnotations;

namespace BaruchsTreks.Enums
{
    public enum FerrataGradeEnum
    {
        [Display(Name = "none")]
        none,

        [Display(Name = "A")]
        A,

        [Display(Name = "A/B")]
        AB,
        
        [Display(Name = "B")]
        B,

        [Display(Name = "B/C")]
        BC,

        [Display(Name = "C")]
        C,

        [Display(Name = "C/D")]
        CD,

        [Display(Name = "D")]
        D,

        [Display(Name = "D/E")]
        DE,

        [Display(Name = "E")]
        E,

        [Display(Name = "F")]
        F
    }
}
