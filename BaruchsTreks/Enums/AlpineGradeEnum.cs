using System.ComponentModel.DataAnnotations;

namespace BaruchsTreks.Enums
{
    public enum AlpineGradeEnum
    {
        [Display(Name = "none")]
        none,

        [Display(Name = "F")]
        F,

        [Display(Name = "PD-")]
        PDMinus,

        [Display(Name = "PD")]
        PD,

        [Display(Name = "PD+")]
        PDPlus,

        [Display(Name = "AD-")]
        ADMinus,

        [Display(Name = "AD")]
        AD,

        [Display(Name = "AD+")]
        ADPlus,

        [Display(Name = "D-")]
        DMinus,

        [Display(Name = "D")]
        D,

        [Display(Name = "D+")]
        DPlus,

        [Display(Name = "TD-")]
        TDMinus,

        [Display(Name = "TD")]
        TD,

        [Display(Name = "TD+")]
        TDPlus,

        [Display(Name = "ED-")]
        EDMinus,

        [Display(Name = "ED")]
        ED,

        [Display(Name = "ED+")]
        EDPlus,

        [Display(Name = "EDx")]
        EDx
    }
}
