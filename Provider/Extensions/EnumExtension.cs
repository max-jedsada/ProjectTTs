using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Project.Database.Enum.Extensions
{
    public static class EnumHelper
    {
        public static string GetDisplayName(this System.Enum enumValue)
        {
            return enumValue.GetType()?
                            .GetMember(enumValue.ToString())?
                            .First()?
                            .GetCustomAttribute<DisplayAttribute>()?
                            .Name!;
        }
    }
}
