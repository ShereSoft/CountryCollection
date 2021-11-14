using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ShereSoft
{
    /// <summary>
    /// Initializes a new instance of the ReadOnlyCountryInfo class
    /// </summary>
    public sealed class ReadOnlyCountryInfo
    {
        /// <summary>
        /// ISO English Short Name
        /// </summary>
        public string IsoEnglishShortName { get; }

        /// <summary>
        /// ISO Alpha 2 Code
        /// </summary>
        public string IsoAlpha2Code { get; }

        /// <summary>
        /// ISO Alpha 3 Code
        /// </summary>
        public string IsoAlpha3Code { get; }

        /// <summary>
        /// ISO Numeric Code
        /// </summary>
        public string IsoNumeric { get; }

        /// <summary>
        /// General name
        /// </summary>
        public string Name { get => _name ?? IsoEnglishShortName; set => _name = value; }
        string _name;

        static Dictionary<string, string> GeneralNames { get; set; } = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(culture => new RegionInfo(culture.Name))
            .Where(n => n.TwoLetterISORegionName.Length == 2 && n.TwoLetterISORegionName.All(Char.IsLetter))
            .GroupBy(n => Tuple.Create(n.TwoLetterISORegionName, n.EnglishName))
            .ToDictionary(t => t.Key.Item1, t => t.Key.Item2);

        internal ReadOnlyCountryInfo(string isoAlpha2Code, string isoAlpha3Code, string isoNumeric, string isoName)
        {
            if (isoAlpha2Code == null)
            {
                throw new ArgumentNullException(nameof(isoAlpha2Code));
            }

            if (isoAlpha3Code == null)
            {
                throw new ArgumentNullException(nameof(isoAlpha3Code));
            }

            if (isoNumeric == null)
            {
                throw new ArgumentNullException(nameof(isoNumeric));
            }

            if (isoName == null)
            {
                throw new ArgumentNullException(nameof(isoName));
            }

            if (isoAlpha2Code.Length != 2)
            {
                throw new FormatException(nameof(isoAlpha2Code));
            }

            if (isoAlpha3Code.Length != 3)
            {
                throw new FormatException(nameof(isoAlpha3Code));
            }

            if (isoNumeric.Length != 3 || !isoNumeric.All(Char.IsNumber))
            {
                throw new FormatException(nameof(isoNumeric));
            }

            IsoAlpha2Code = isoAlpha2Code;
            IsoAlpha3Code = isoAlpha3Code;
            IsoNumeric = isoNumeric;
            IsoEnglishShortName = isoName;
            Name = GeneralNames.TryGetValue(isoAlpha2Code, out var name) ? name : isoName;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ReadOnlyCountryInfo ci)
            {
                return ci.IsoAlpha2Code == IsoAlpha2Code && ci.IsoAlpha3Code == IsoAlpha3Code && ci.IsoNumeric == IsoNumeric;
            }
            else if (obj is string c)
            {
                return String.Equals(c, IsoAlpha2Code, StringComparison.OrdinalIgnoreCase) || String.Equals(c, IsoAlpha3Code, StringComparison.OrdinalIgnoreCase) || String.Equals(c.TrimStart('0'), IsoNumeric.TrimStart('0'));
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this country info.
        /// </summary>
        /// <returns>A hash code for the current country info.</returns>
        public override int GetHashCode()
        {
            return IsoAlpha2Code.GetHashCode() ^ IsoAlpha3Code.GetHashCode() ^ IsoNumeric.GetHashCode();
        }
    }
}
