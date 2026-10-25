using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace t2pryct.Models;

public static class EnumExtensions
{
    public static string NombreVisible(this Enum valor)
    {
        var miembro = valor.GetType().GetMember(valor.ToString()).FirstOrDefault();
        return miembro?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? valor.ToString();
    }
}
