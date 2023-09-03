using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BaruchsTreks.Enums
{

    public static class EnumExtensions
    {
        public static string DisplayNameForEnum(this Enum value)
        {
            var displayAttribute = value
                .GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()
                ?.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.GetName() ?? value.ToString();
        }
    }
}
