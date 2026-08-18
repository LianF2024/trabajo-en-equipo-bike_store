using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace BikeStore.Web.ModelBinding;

public static class FormValueParser
{
    public static string Text(IFormCollection form, string key)
        => form[key].ToString().Trim();

    public static bool TryInt(IFormCollection form, string key, out int value)
        => int.TryParse(Text(form, key), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

    public static bool IsTrue(IFormCollection form, string key)
        => form[key].Any(value => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase));

    public static bool TryDecimal(IFormCollection form, string key, out decimal value)
    {
        value = 0;
        var normalized = Text(form, key).Replace(" ", string.Empty);
        if (normalized.Length == 0) return false;

        var lastComma = normalized.LastIndexOf(',');
        var lastDot = normalized.LastIndexOf('.');

        if (lastComma >= 0 && lastDot >= 0)
        {
            normalized = lastComma > lastDot
                ? normalized.Replace(".", string.Empty).Replace(',', '.')
                : normalized.Replace(",", string.Empty);
        }
        else if (lastComma >= 0)
        {
            normalized = normalized.Replace(',', '.');
        }

        return decimal.TryParse(
            normalized,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out value);
    }
}
