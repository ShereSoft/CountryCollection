using ShereSoft;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
/// Provides the class for a pre-loaded, read-only country collection or a customizable country list instance.
/// </summary>
public sealed class CountryCollection : IEnumerable<ReadOnlyCountryInfo>
{
    static ReadOnlyCountryInfo[] CountriesCache = null;
    static Dictionary<string, ReadOnlyCountryInfo> Dict = new Dictionary<string, ReadOnlyCountryInfo>(new CountryCodeComparer());
    static Dictionary<int, ReadOnlyCountryInfo> NumericIntDict = null;

    internal List<ReadOnlyCountryInfo> _countries = null;
    internal Dictionary<string, ReadOnlyCountryInfo> _dict = new Dictionary<string, ReadOnlyCountryInfo>(new CountryCodeComparer());
    internal Dictionary<int, ReadOnlyCountryInfo> _numericIntDict = null;

    static CountryCollection()
    {
        CountriesCache = new ReadOnlyCountryInfo[Data.Length];

        for (int i = 0; i < Data.Length; i++)
        {
            var values = Data[i];
            CountriesCache[i] = new ReadOnlyCountryInfo(values[0], values[1], values[2], values[3]);
        }

        NumericIntDict = CountriesCache.ToDictionary(c => int.Parse(c.IsoNumeric), c => c);

        foreach (var c in CountriesCache)
        {
            Dict.Add(c.IsoAlpha2Code, c);
        }

        foreach (var c in CountriesCache)
        {
            Dict.Add(c.IsoAlpha3Code, c);
        }

        foreach (var c in CountriesCache)
        {
            Dict.Add(c.IsoNumeric, c);
        }
    }

    /// <summary>
    /// Initializes a new instance of the CountryCollection class that contains countries copied from the static CountryCollection object with add/remove capability
    /// </summary>
    public CountryCollection()
    {
        _countries = CountriesCache.ToList(); 
        _dict = new Dictionary<string, ReadOnlyCountryInfo>(Dict, new CountryCodeComparer());
        _numericIntDict = CountriesCache.ToDictionary(c => int.Parse(c.IsoNumeric), c => c);
    }

    /// <summary>
    /// Adds a country
    /// </summary>
    /// <param name="isoAlpha2Code">The ISO aplpha 2 code</param>
    /// <param name="isoAlpha3Code">The ISO aplpha 3 code</param>
    /// <param name="isoNumeric">The ISO numeric code</param>
    /// <param name="isoName">The country name</param>
    public void Add(string isoAlpha2Code, string isoAlpha3Code, string isoNumeric, string isoName)
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

        var ci = new ReadOnlyCountryInfo(isoAlpha2Code, isoAlpha3Code, isoNumeric, isoName);

        if (_countries.Any(c => c.Equals(isoAlpha2Code) || c.Equals(isoAlpha3Code) || c.Equals(isoNumeric)))
        {
            throw new ArgumentException("Country already exists.");
        }

        _countries.Add(ci);
        _dict.Add(ci.IsoAlpha2Code, ci);
        _dict.Add(ci.IsoAlpha3Code, ci);
        _dict.Add(ci.IsoNumeric, ci);
        _numericIntDict.Add(int.Parse(ci.IsoNumeric), ci);
    }

    /// <summary>
    /// Removes the country by code
    /// </summary>
    /// <param name="code">The country code to remove from the current instance</param>
    public void Remove(string code)
    {
        if (code == null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        var ci = _countries.FirstOrDefault(c => c.Equals(code));

        if (ci != null)
        {
            _countries.Remove(ci);
            _dict.Remove(ci.IsoAlpha2Code);
            _dict.Remove(ci.IsoAlpha3Code);
            _dict.Remove(ci.IsoNumeric);
            _numericIntDict.Remove(int.Parse(ci.IsoNumeric));
        }
    }

    /// <summary>
    /// Returns a matching country by code
    /// </summary>
    /// <param name="code">The value to find in the CountryCollection.</param>
    /// <returns>The matching country in the CountryCollection.</returns>
    public static ReadOnlyCountryInfo GetCountry(string code)
    {
        if (code == null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        Dict.TryGetValue(code, out var ci);

        return ci;
    }

    /// <summary>
    /// Returns a matching country by code
    /// </summary>
    /// <param name="code">The value to find in the CountryCollection.</param>
    /// <returns>The matching country in the CountryCollection.</returns>
    public static ReadOnlyCountryInfo GetCountry(int code)
    {
        return NumericIntDict.TryGetValue(code, out var ci) ? ci : null;
    }

    /// <summary>
    /// Determines whether CountryCollection contains a specified code
    /// </summary>
    /// <param name="code">The value to check in the CountryCollection.</param>
    /// <returns>true if the CountryCollection contains a country that has the specified code; otherwise, false.</returns>
    public static bool Contains(string code)
    {
        return GetCountry(code) != null;
    }

    /// <summary>
    /// Determines whether CountryCollection contains a specified code
    /// </summary>
    /// <param name="code">The value to check in the CountryCollection.</param>
    /// <returns>true if the CountryCollection contains a country that has the specified code; otherwise, false.</returns>
    public static bool Contains(int code)
    {
        return GetCountry(code) != null;
    }

    /// <summary>
    /// Returns a normalized value from the matching country code.
    /// </summary>
    /// <param name="code">The value to normalize</param>
    /// <returns>The normalized code in the CountryCollection.</returns>
#if NET40
    public static string Normalize(string code)
#else
    public static string Normalize([DisallowNull] string code)
#endif

    {
        var ci = GetCountry(code);
        
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
    /// <param name="code">The value to normalize</param>
    /// <returns>The normalized code in the CountryCollection.</returns>
    public static string Normalize(int code)
    {
        var ci = GetCountry(code);

        return ci != null ? ci.IsoNumeric : null;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<ReadOnlyCountryInfo> GetEnumerator()
    {
        return ((IEnumerable<ReadOnlyCountryInfo>)_countries).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _countries.GetEnumerator();
    }

    static string[][] Data = new[]
    {
        new[] {"AD","AND","020","Andorra"},
        new[] {"AE","ARE","784","United Arab Emirates (the)"},
        new[] {"AF","AFG","004","Afghanistan"},
        new[] {"AG","ATG","028","Antigua and Barbuda"},
        new[] {"AI","AIA","660","Anguilla"},
        new[] {"AL","ALB","008","Albania"},
        new[] {"AM","ARM","051","Armenia"},
        new[] {"AO","AGO","024","Angola"},
        new[] {"AQ","ATA","010","Antarctica"},
        new[] {"AR","ARG","032","Argentina"},
        new[] {"AS","ASM","016","American Samoa"},
        new[] {"AT","AUT","040","Austria"},
        new[] {"AU","AUS","036","Australia"},
        new[] {"AW","ABW","533","Aruba"},
        new[] {"AX","ALA","248","Åland Islands"},
        new[] {"AZ","AZE","031","Azerbaijan"},
        new[] {"BA","BIH","070","Bosnia and Herzegovina"},
        new[] {"BB","BRB","052","Barbados"},
        new[] {"BD","BGD","050","Bangladesh"},
        new[] {"BE","BEL","056","Belgium"},
        new[] {"BF","BFA","854","Burkina Faso"},
        new[] {"BG","BGR","100","Bulgaria"},
        new[] {"BH","BHR","048","Bahrain"},
        new[] {"BI","BDI","108","Burundi"},
        new[] {"BJ","BEN","204","Benin"},
        new[] {"BL","BLM","652","Saint Barthélemy"},
        new[] {"BM","BMU","060","Bermuda"},
        new[] {"BN","BRN","096","Brunei Darussalam"},
        new[] {"BO","BOL","068","Bolivia (Plurinational State of)"},
        new[] {"BQ","BES","535","Bonaire, Sint Eustatius and Saba"},
        new[] {"BR","BRA","076","Brazil"},
        new[] {"BS","BHS","044","Bahamas (the)"},
        new[] {"BT","BTN","064","Bhutan"},
        new[] {"BV","BVT","074","Bouvet Island"},
        new[] {"BW","BWA","072","Botswana"},
        new[] {"BY","BLR","112","Belarus"},
        new[] {"BZ","BLZ","084","Belize"},
        new[] {"CA","CAN","124","Canada"},
        new[] {"CC","CCK","166","Cocos (Keeling) Islands (the)"},
        new[] {"CD","COD","180","Congo (the Democratic Republic of the)"},
        new[] {"CF","CAF","140","Central African Republic (the)"},
        new[] {"CG","COG","178","Congo (the)"},
        new[] {"CH","CHE","756","Switzerland"},
        new[] {"CI","CIV","384","Côte d'Ivoire"},
        new[] {"CK","COK","184","Cook Islands (the)"},
        new[] {"CL","CHL","152","Chile"},
        new[] {"CM","CMR","120","Cameroon"},
        new[] {"CN","CHN","156","China"},
        new[] {"CO","COL","170","Colombia"},
        new[] {"CR","CRI","188","Costa Rica"},
        new[] {"CU","CUB","192","Cuba"},
        new[] {"CV","CPV","132","Cabo Verde"},
        new[] {"CW","CUW","531","Curaçao"},
        new[] {"CX","CXR","162","Christmas Island"},
        new[] {"CY","CYP","196","Cyprus"},
        new[] {"CZ","CZE","203","Czechia"},
        new[] {"DE","DEU","276","Germany"},
        new[] {"DJ","DJI","262","Djibouti"},
        new[] {"DK","DNK","208","Denmark"},
        new[] {"DM","DMA","212","Dominica"},
        new[] {"DO","DOM","214","Dominican Republic (the)"},
        new[] {"DZ","DZA","012","Algeria"},
        new[] {"EC","ECU","218","Ecuador"},
        new[] {"EE","EST","233","Estonia"},
        new[] {"EG","EGY","818","Egypt"},
        new[] {"EH","ESH","732","Western Sahara*"},
        new[] {"ER","ERI","232","Eritrea"},
        new[] {"ES","ESP","724","Spain"},
        new[] {"ET","ETH","231","Ethiopia"},
        new[] {"FI","FIN","246","Finland"},
        new[] {"FJ","FJI","242","Fiji"},
        new[] {"FK","FLK","238","Falkland Islands (the) [Malvinas]"},
        new[] {"FM","FSM","583","Micronesia (Federated States of)"},
        new[] {"FO","FRO","234","Faroe Islands (the)"},
        new[] {"FR","FRA","250","France"},
        new[] {"GA","GAB","266","Gabon"},
        new[] {"GB","GBR","826","United Kingdom of Great Britain and Northern Ireland (the)"},
        new[] {"GD","GRD","308","Grenada"},
        new[] {"GE","GEO","268","Georgia"},
        new[] {"GF","GUF","254","French Guiana"},
        new[] {"GG","GGY","831","Guernsey"},
        new[] {"GH","GHA","288","Ghana"},
        new[] {"GI","GIB","292","Gibraltar"},
        new[] {"GL","GRL","304","Greenland"},
        new[] {"GM","GMB","270","Gambia (the)"},
        new[] {"GN","GIN","324","Guinea"},
        new[] {"GP","GLP","312","Guadeloupe"},
        new[] {"GQ","GNQ","226","Equatorial Guinea"},
        new[] {"GR","GRC","300","Greece"},
        new[] {"GS","SGS","239","South Georgia and the South Sandwich Islands"},
        new[] {"GT","GTM","320","Guatemala"},
        new[] {"GU","GUM","316","Guam"},
        new[] {"GW","GNB","624","Guinea-Bissau"},
        new[] {"GY","GUY","328","Guyana"},
        new[] {"HK","HKG","344","Hong Kong"},
        new[] {"HM","HMD","334","Heard Island and McDonald Islands"},
        new[] {"HN","HND","340","Honduras"},
        new[] {"HR","HRV","191","Croatia"},
        new[] {"HT","HTI","332","Haiti"},
        new[] {"HU","HUN","348","Hungary"},
        new[] {"ID","IDN","360","Indonesia"},
        new[] {"IE","IRL","372","Ireland"},
        new[] {"IL","ISR","376","Israel"},
        new[] {"IM","IMN","833","Isle of Man"},
        new[] {"IN","IND","356","India"},
        new[] {"IO","IOT","086","British Indian Ocean Territory (the)"},
        new[] {"IQ","IRQ","368","Iraq"},
        new[] {"IR","IRN","364","Iran (Islamic Republic of)"},
        new[] {"IS","ISL","352","Iceland"},
        new[] {"IT","ITA","380","Italy"},
        new[] {"JE","JEY","832","Jersey"},
        new[] {"JM","JAM","388","Jamaica"},
        new[] {"JO","JOR","400","Jordan"},
        new[] {"JP","JPN","392","Japan"},
        new[] {"KE","KEN","404","Kenya"},
        new[] {"KG","KGZ","417","Kyrgyzstan"},
        new[] {"KH","KHM","116","Cambodia"},
        new[] {"KI","KIR","296","Kiribati"},
        new[] {"KM","COM","174","Comoros (the)"},
        new[] {"KN","KNA","659","Saint Kitts and Nevis"},
        new[] {"KP","PRK","408","Korea (the Democratic People's Republic of)"},
        new[] {"KR","KOR","410","Korea (the Republic of)"},
        new[] {"KW","KWT","414","Kuwait"},
        new[] {"KY","CYM","136","Cayman Islands (the)"},
        new[] {"KZ","KAZ","398","Kazakhstan"},
        new[] {"LA","LAO","418","Lao People's Democratic Republic (the)"},
        new[] {"LB","LBN","422","Lebanon"},
        new[] {"LC","LCA","662","Saint Lucia"},
        new[] {"LI","LIE","438","Liechtenstein"},
        new[] {"LK","LKA","144","Sri Lanka"},
        new[] {"LR","LBR","430","Liberia"},
        new[] {"LS","LSO","426","Lesotho"},
        new[] {"LT","LTU","440","Lithuania"},
        new[] {"LU","LUX","442","Luxembourg"},
        new[] {"LV","LVA","428","Latvia"},
        new[] {"LY","LBY","434","Libya"},
        new[] {"MA","MAR","504","Morocco"},
        new[] {"MC","MCO","492","Monaco"},
        new[] {"MD","MDA","498","Moldova (the Republic of)"},
        new[] {"ME","MNE","499","Montenegro"},
        new[] {"MF","MAF","663","Saint Martin (French part)"},
        new[] {"MG","MDG","450","Madagascar"},
        new[] {"MH","MHL","584","Marshall Islands (the)"},
        new[] {"MK","MKD","807","North Macedonia"},
        new[] {"ML","MLI","466","Mali"},
        new[] {"MM","MMR","104","Myanmar"},
        new[] {"MN","MNG","496","Mongolia"},
        new[] {"MO","MAC","446","Macao"},
        new[] {"MP","MNP","580","Northern Mariana Islands (the)"},
        new[] {"MQ","MTQ","474","Martinique"},
        new[] {"MR","MRT","478","Mauritania"},
        new[] {"MS","MSR","500","Montserrat"},
        new[] {"MT","MLT","470","Malta"},
        new[] {"MU","MUS","480","Mauritius"},
        new[] {"MV","MDV","462","Maldives"},
        new[] {"MW","MWI","454","Malawi"},
        new[] {"MX","MEX","484","Mexico"},
        new[] {"MY","MYS","458","Malaysia"},
        new[] {"MZ","MOZ","508","Mozambique"},
        new[] {"NA","NAM","516","Namibia"},
        new[] {"NC","NCL","540","New Caledonia"},
        new[] {"NE","NER","562","Niger (the)"},
        new[] {"NF","NFK","574","Norfolk Island"},
        new[] {"NG","NGA","566","Nigeria"},
        new[] {"NI","NIC","558","Nicaragua"},
        new[] {"NL","NLD","528","Netherlands (the)"},
        new[] {"NO","NOR","578","Norway"},
        new[] {"NP","NPL","524","Nepal"},
        new[] {"NR","NRU","520","Nauru"},
        new[] {"NU","NIU","570","Niue"},
        new[] {"NZ","NZL","554","New Zealand"},
        new[] {"OM","OMN","512","Oman"},
        new[] {"PA","PAN","591","Panama"},
        new[] {"PE","PER","604","Peru"},
        new[] {"PF","PYF","258","French Polynesia"},
        new[] {"PG","PNG","598","Papua New Guinea"},
        new[] {"PH","PHL","608","Philippines (the)"},
        new[] {"PK","PAK","586","Pakistan"},
        new[] {"PL","POL","616","Poland"},
        new[] {"PM","SPM","666","Saint Pierre and Miquelon"},
        new[] {"PN","PCN","612","Pitcairn"},
        new[] {"PR","PRI","630","Puerto Rico"},
        new[] {"PS","PSE","275","Palestine, State of"},
        new[] {"PT","PRT","620","Portugal"},
        new[] {"PW","PLW","585","Palau"},
        new[] {"PY","PRY","600","Paraguay"},
        new[] {"QA","QAT","634","Qatar"},
        new[] {"RE","REU","638","Réunion"},
        new[] {"RO","ROU","642","Romania"},
        new[] {"RS","SRB","688","Serbia"},
        new[] {"RU","RUS","643","Russian Federation (the)"},
        new[] {"RW","RWA","646","Rwanda"},
        new[] {"SA","SAU","682","Saudi Arabia"},
        new[] {"SB","SLB","090","Solomon Islands"},
        new[] {"SC","SYC","690","Seychelles"},
        new[] {"SD","SDN","729","Sudan (the)"},
        new[] {"SE","SWE","752","Sweden"},
        new[] {"SG","SGP","702","Singapore"},
        new[] {"SH","SHN","654","Saint Helena, Ascension and Tristan da Cunha"},
        new[] {"SI","SVN","705","Slovenia"},
        new[] {"SJ","SJM","744","Svalbard and Jan Mayen"},
        new[] {"SK","SVK","703","Slovakia"},
        new[] {"SL","SLE","694","Sierra Leone"},
        new[] {"SM","SMR","674","San Marino"},
        new[] {"SN","SEN","686","Senegal"},
        new[] {"SO","SOM","706","Somalia"},
        new[] {"SR","SUR","740","Suriname"},
        new[] {"SS","SSD","728","South Sudan"},
        new[] {"ST","STP","678","Sao Tome and Principe"},
        new[] {"SV","SLV","222","El Salvador"},
        new[] {"SX","SXM","534","Sint Maarten (Dutch part)"},
        new[] {"SY","SYR","760","Syrian Arab Republic (the)"},
        new[] {"SZ","SWZ","748","Eswatini"},
        new[] {"TC","TCA","796","Turks and Caicos Islands (the)"},
        new[] {"TD","TCD","148","Chad"},
        new[] {"TF","ATF","260","French Southern Territories (the)"},
        new[] {"TG","TGO","768","Togo"},
        new[] {"TH","THA","764","Thailand"},
        new[] {"TJ","TJK","762","Tajikistan"},
        new[] {"TK","TKL","772","Tokelau"},
        new[] {"TL","TLS","626","Timor-Leste"},
        new[] {"TM","TKM","795","Turkmenistan"},
        new[] {"TN","TUN","788","Tunisia"},
        new[] {"TO","TON","776","Tonga"},
        new[] {"TR","TUR","792","Turkey"},
        new[] {"TT","TTO","780","Trinidad and Tobago"},
        new[] {"TV","TUV","798","Tuvalu"},
        new[] {"TW","TWN","158","Taiwan (Province of China)"},
        new[] {"TZ","TZA","834","Tanzania, the United Republic of"},
        new[] {"UA","UKR","804","Ukraine"},
        new[] {"UG","UGA","800","Uganda"},
        new[] {"UM","UMI","581","United States Minor Outlying Islands (the)"},
        new[] {"US","USA","840","United States of America (the)"},
        new[] {"UY","URY","858","Uruguay"},
        new[] {"UZ","UZB","860","Uzbekistan"},
        new[] {"VA","VAT","336","Holy See (the)"},
        new[] {"VC","VCT","670","Saint Vincent and the Grenadines"},
        new[] {"VE","VEN","862","Venezuela (Bolivarian Republic of)"},
        new[] {"VG","VGB","092","Virgin Islands (British)"},
        new[] {"VI","VIR","850","Virgin Islands (U.S.)"},
        new[] {"VN","VNM","704","Viet Nam"},
        new[] {"VU","VUT","548","Vanuatu"},
        new[] {"WF","WLF","876","Wallis and Futuna"},
        new[] {"WS","WSM","882","Samoa"},
        new[] {"YE","YEM","887","Yemen"},
        new[] {"YT","MYT","175","Mayotte"},
        new[] {"ZA","ZAF","710","South Africa"},
        new[] {"ZM","ZMB","894","Zambia"},
        new[] {"ZW","ZWE","716","Zimbabwe"},
    };

    class CountryCodeComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (x.Length == y.Length)
            {
                return x.Equals(y, StringComparison.OrdinalIgnoreCase);
            }

            if (Char.IsNumber(x[0]))
            {
                int i = 0;
                for (; i < x.Length; i++)
                {
                    if (x[i] != '0')
                    {
                        break;
                    }
                }

                int j = 0;
                for (; j < y.Length; j++)
                {
                    if (y[j] != '0')
                    {
                        break;
                    }
                }

                if (x.Length - i != y.Length - j)
                {
                    return false;
                }

                for (; i < x.Length; i++, j++)
                {
                    if (x[i] != y[j])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public int GetHashCode(string obj)

        {
            int hashcode = 0;

            if (Char.IsNumber(obj[0]))
            {
                int i = 0;
                for (; i < obj.Length; i++)
                {
                    if (obj[i] != '0')
                    {
                        break;
                    }
                }

                for (; i < obj.Length; i++)
                {
                    hashcode ^= obj[i].GetHashCode();
                }
            }
            else
            {
                for (int i = 0; i < obj.Length; i++)
                {
                    char c = obj[i];

                    if (c <= 'Z')
                    {
                        hashcode ^= c.GetHashCode();
                    }
                    else
                    {
                        hashcode ^= Char.ToUpper(c).GetHashCode();
                    }
                }
            }

            return hashcode;
        }
    }
}