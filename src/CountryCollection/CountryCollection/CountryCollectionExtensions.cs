using ShereSoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Provides a set of static (Shared in Visual Basic) methods for querying a CountryCollection instance
/// </summary>
public static class CountryCollectionExtensions
{
    /// <summary>
    /// Returns a matching country by code
    /// </summary>
    /// <param name="source">The CountryCollection</param>
    /// <param name="code">The value to find in the CountryCollection.</param>
    /// <returns>The matching country in the CountryCollection.</returns>
    public static ReadOnlyCountryInfo GetCountry(this CountryCollection source, string code)
    {
        new string[0].FirstOrDefault();
        if (code == null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        source._dict.TryGetValue(code, out var ci);

        return ci;
    }

    /// <summary>
    /// Returns a matching country by code
    /// </summary>
    /// <param name="source">The CountryCollection</param>
    /// <param name="code">The value to find in the CountryCollection.</param>
    /// <returns>The matching country in the CountryCollection.</returns>
    public static ReadOnlyCountryInfo GetCountry(this CountryCollection source, int code)
    {
        return source._numericIntDict.TryGetValue(code, out var ci) ? ci : null;
    }

    /// <summary>
    /// Determines whether CountryCollection contains a specified code
    /// </summary>
    /// <param name="source">The CountryCollection</param>
    /// <param name="code">The value to check in the CountryCollection.</param>
    /// <returns>true if the CountryCollection contains a country that has the specified code; otherwise, false.</returns>
    public static bool Contains(this CountryCollection source, string code)
    {
        return GetCountry(source, code) != null;
    }

    /// <summary>
    /// Determines whether CountryCollection contains a specified code.
    /// </summary>
    /// <param name="source">The CountryCollection</param>
    /// <param name="code">The value to check in the CountryCollection.</param>
    /// <returns>true if the CountryCollection contains a country that has the specified code; otherwise, false.</returns>
    public static bool Contains(this CountryCollection source, int code)
    {
        return GetCountry(source, code) != null;
    }

    /// <summary>
    /// Returns a normalized value from the matching country code.
    /// </summary>
    /// <param name="source">The CountryCollection</param>
    /// <param name="code">The value to normalize</param>
    /// <returns>The normalized code in the CountryCollection.</returns>
    public static string Normalize(this CountryCollection source, string code)
    {
        var ci = GetCountry(source, code);

        if (ci != null)
        {
            if (code.All(Char.IsNumber))
            {
                return ci.IsoNumeric;
            }
            else if (code.Length == 2)
            {
                return ci.IsoAlpha2Code;
            }
            else if (code.Length == 3)
            {
                return ci.IsoAlpha3Code;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns a normalized value from the matching country code.
    /// </summary>
    /// <param name="source">The CountryCollection</param>
    /// <param name="code">A value to normalize</param>
    /// <returns>The normalized code in the CountryCollection.</returns>
    public static string Normalize(this CountryCollection source, int code)
    {
        var ci = GetCountry(source, code);

        return ci != null ? ci.IsoNumeric : null;
    }
}