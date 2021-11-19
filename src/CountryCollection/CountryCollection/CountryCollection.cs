using ShereSoft;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
/// Provides the class for a pre-loaded, read-only country collection or a customizable country list instance.
/// </summary>
public sealed class CountryCollection : IEnumerable<CountryInfo>
{
    static CountryInfo[] CountriesCache = null;
    static Dictionary<string, CountryInfo> Dict = new Dictionary<string, CountryInfo>(new CountryCodeComparer());
    static Dictionary<int, CountryInfo> NumericIntDict = null;

    internal List<CountryInfo> _countries = null;
    internal Dictionary<string, CountryInfo> _dict = new Dictionary<string, CountryInfo>(new CountryCodeComparer());
    internal Dictionary<int, CountryInfo> _numericIntDict = null;

    /// <summary>
    ///  Gets a collection containing all the countries
    /// </summary>
    public static IEnumerable<CountryInfo> Values => CountriesCache;

    static CountryCollection()
    {
        CountriesCache = new CountryInfo[Data.Length];

        for (int i = 0; i < Data.Length; i++)
        {
            var values = Data[i];
            CountriesCache[i] = new CountryInfo(values[0], values[1], values[2], values[3], values[5], values[4]);
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
        _dict = new Dictionary<string, CountryInfo>(Dict, new CountryCodeComparer());
        _numericIntDict = CountriesCache.ToDictionary(c => int.Parse(c.IsoNumeric), c => c);
    }

    /// <summary>
    /// Returns the matching country by code; null if not found
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public CountryInfo this[string code]
    {
        get
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            _dict.TryGetValue(code, out var ci);

            return ci;
        }
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
        Add(isoAlpha2Code, isoAlpha3Code, isoNumeric, isoName, null, null);
    }

    /// <summary>
    /// Adds a country
    /// </summary>
    /// <param name="isoAlpha2Code">The ISO aplpha 2 code</param>
    /// <param name="isoAlpha3Code">The ISO aplpha 3 code</param>
    /// <param name="isoNumeric">The ISO numeric code</param>
    /// <param name="isoName">The country name</param>
    /// <param name="generalName">General name</param>
    /// <param name="fullName">Full name</param>
    public void Add(string isoAlpha2Code, string isoAlpha3Code, string isoNumeric, string isoName, string generalName, string fullName)
    {
        var ci = new CountryInfo(isoAlpha2Code, isoAlpha3Code, isoNumeric, isoName, generalName, fullName);  // null checked here

        if (_countries.Any(c => c.Equals(isoAlpha2Code) || c.Equals(isoAlpha3Code) || c.Equals(isoNumeric)))
        {
            throw new ArgumentException("Country Code already exists.");
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
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<CountryInfo> GetEnumerator()
    {
        return ((IEnumerable<CountryInfo>)_countries).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _countries.GetEnumerator();
    }

    /// <summary>
    /// Returns a matching country by code
    /// </summary>
    /// <param name="code">The value to find in the CountryCollection.</param>
    /// <returns>The matching country in the CountryCollection.</returns>
    public static CountryInfo GetCountry(string code)
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
    public static CountryInfo GetCountry(int code)
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
        var ci = GetCountry(code);  // null checked here
        
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

    static string[][] Data = new[]
    {
        new[] {"AD","AND","020","Andorra", "The Principality of Andorra", "Andorra" },
        new[] {"AE","ARE","784","United Arab Emirates (the)", "The United Arab Emirates", "United Arab Emirates" },
        new[] {"AF","AFG","004","Afghanistan", "The Islamic Emirate of Afghanistan", "Afghanistan" },
        new[] {"AG","ATG","028","Antigua and Barbuda", "Antigua and Barbuda", "Antigua and Barbuda" },
        new[] {"AI","AIA","660","Anguilla", "Anguilla", "Anguilla" },
        new[] {"AL","ALB","008","Albania", "The Republic of Albania", "Albania" },
        new[] {"AM","ARM","051","Armenia", "The Republic of Armenia", "Armenia" },
        new[] {"AO","AGO","024","Angola", "The Republic of Angola", "Angola" },
        new[] {"AQ","ATA","010","Antarctica", "Antarctica", "Antarctica" },
        new[] {"AR","ARG","032","Argentina", "The Argentine Republic", "Argentina" },
        new[] {"AS","ASM","016","American Samoa", "American Samoa", "American Samoa" },
        new[] {"AT","AUT","040","Austria", "The Republic of Austria", "Austria" },
        new[] {"AU","AUS","036","Australia", "The Commonwealth of Australia", "Australia" },
        new[] {"AW","ABW","533","Aruba", "Aruba", "Aruba" },
        new[] {"AX","ALA","248","Åland Islands", "The Åland Islands", "Åland" },
        new[] {"AZ","AZE","031","Azerbaijan", "The Republic of Azerbaijan", "Azerbaijan" },
        new[] {"BA","BIH","070","Bosnia and Herzegovina", "Bosnia and Herzegovina", "Bosnia" },
        new[] {"BB","BRB","052","Barbados", "Barbados", "Barbados" },
        new[] {"BD","BGD","050","Bangladesh", "The People's Republic of Bangladesh", "Bangladesh" },
        new[] {"BE","BEL","056","Belgium", "The Kingdom of Belgium", "Belgium" },
        new[] {"BF","BFA","854","Burkina Faso", "Burkina Faso", "Burkina Faso" },
        new[] {"BG","BGR","100","Bulgaria", "The Republic of Bulgaria", "Bulgaria" },
        new[] {"BH","BHR","048","Bahrain", "The Kingdom of Bahrain", "Bahrain" },
        new[] {"BI","BDI","108","Burundi", "The Republic of Burundi", "Burundi" },
        new[] {"BJ","BEN","204","Benin", "The Republic of Benin", "Benin" },
        new[] {"BL","BLM","652","Saint Barthélemy", "The Territorial Collectivity of Saint Barthélemy", "Saint Barthélemy" },
        new[] {"BM","BMU","060","Bermuda", "Bermuda", "Bermuda" },
        new[] {"BN","BRN","096","Brunei Darussalam", "The Nation of Brunei, the Abode of Peace", "Brunei" },
        new[] {"BO","BOL","068","Bolivia (Plurinational State of)", "The Plurinational State of Bolivia", "Bolivia" },
        new[] {"BQ","BES","535","Bonaire, Sint Eustatius and Saba", "The Public Body of Bonaire", "Bonaire" },
        new[] {"BR","BRA","076","Brazil", "The Federative Republic of Brazil", "Brazil" },
        new[] {"BS","BHS","044","Bahamas (the)", "The Commonwealth of the Bahamas", "The Bahamas" },
        new[] {"BT","BTN","064","Bhutan", "The Kingdom of Bhutan", "Bhutan" },
        new[] {"BV","BVT","074","Bouvet Island", "Bouvet Island", "Bouvet Island" },
        new[] {"BW","BWA","072","Botswana", "The Republic of Botswana", "Botswana" },
        new[] {"BY","BLR","112","Belarus", "The Republic of Belarus", "Belarus" },
        new[] {"BZ","BLZ","084","Belize", "Belize", "Belize" },
        new[] {"CA","CAN","124","Canada", "Canada", "Canada" },
        new[] {"CC","CCK","166","Cocos (Keeling) Islands (the)", "The Territory of Cocos (Keeling) Islands", "Cocos (Keeling) Islands" },
        new[] {"CD","COD","180","Congo (the Democratic Republic of the)", "The Democratic Republic of the Congo", "Democratic Republic of the Congo" },
        new[] {"CF","CAF","140","Central African Republic (the)", "The Central African Republic", "Central African Republic" },
        new[] {"CG","COG","178","Congo (the)", "The Republic of the Congo", "Congo Republic" },
        new[] {"CH","CHE","756","Switzerland", "The Swiss Confederation", "Switzerland" },
        new[] {"CI","CIV","384","Côte d'Ivoire", "The Republic of Côte d'Ivoire", "Côte d'Ivoire" },
        new[] {"CK","COK","184","Cook Islands (the)", "The Cook Islands", "Cook Islands" },
        new[] {"CL","CHL","152","Chile", "The Republic of Chile", "Chile" },
        new[] {"CM","CMR","120","Cameroon", "The Republic of Cameroon", "Cameroon" },
        new[] {"CN","CHN","156","China", "The People's Republic of China", "China" },
        new[] {"CO","COL","170","Colombia", "The Republic of Colombia", "Colombia" },
        new[] {"CR","CRI","188","Costa Rica", "The Republic of Costa Rica", "Costa Rica" },
        new[] {"CU","CUB","192","Cuba", "The Republic of Cuba", "Cuba" },
        new[] {"CV","CPV","132","Cabo Verde", "The Republic of Cabo Verde", "Cape Verde" },
        new[] {"CW","CUW","531","Curaçao", "The Country of Curaçao", "Curaçao" },
        new[] {"CX","CXR","162","Christmas Island", "The Territory of Christmas Island", "Christmas Island" },
        new[] {"CY","CYP","196","Cyprus", "The Republic of Cyprus", "Cyprus" },
        new[] {"CZ","CZE","203","Czechia", "The Czech Republic", "Czechia" },
        new[] {"DE","DEU","276","Germany", "The Federal Republic of Germany", "Germany" },
        new[] {"DJ","DJI","262","Djibouti", "The Republic of Djibouti", "Djibouti" },
        new[] {"DK","DNK","208","Denmark", "The Kingdom of Denmark", "Denmark" },
        new[] {"DM","DMA","212","Dominica", "The Commonwealth of Dominica", "Dominica" },
        new[] {"DO","DOM","214","Dominican Republic (the)", "The Dominican Republic", "Dominican Republic" },
        new[] {"DZ","DZA","012","Algeria", "The People's Democratic Republic of Algeria", "Algeria" },
        new[] {"EC","ECU","218","Ecuador", "The Republic of Ecuador", "Ecuador" },
        new[] {"EE","EST","233","Estonia", "The Republic of Estonia", "Estonia" },
        new[] {"EG","EGY","818","Egypt", "The Arab Republic of Egypt", "Egypt" },
        new[] {"EH","ESH","732","Western Sahara", "Western Sahara", "Western Sahara" },
        new[] {"ER","ERI","232","Eritrea", "The State of Eritrea", "Eritrea" },
        new[] {"ES","ESP","724","Spain", "The Kingdom of Spain", "Spain" },
        new[] {"ET","ETH","231","Ethiopia", "The Federal Democratic Republic of Ethiopia", "Ethiopia" },
        new[] {"FI","FIN","246","Finland", "The Republic of Finland", "Finland" },
        new[] {"FJ","FJI","242","Fiji", "The Republic of Fiji", "Fiji" },
        new[] {"FK","FLK","238","Falkland Islands (the) [Malvinas]", "", "Falkland Islands" },
        new[] {"FM","FSM","583","Micronesia (Federated States of)", "the Federated States of Micronesia", "Micronesia" },
        new[] {"FO","FRO","234","Faroe Islands (the)", "The Falkland Islands", "Faroe Islands" },
        new[] {"FR","FRA","250","France", "The French Republic", "France" },
        new[] {"GA","GAB","266","Gabon", "The Gabonese Republic", "Gabon" },
        new[] {"GB","GBR","826","United Kingdom of Great Britain and Northern Ireland (the)", "The United Kingdom of Great Britain and Northern Ireland", "United Kingdom" },
        new[] {"GD","GRD","308","Grenada", "Grenada", "Grenada" },
        new[] {"GE","GEO","268","Georgia", "Georgia ", "Georgia" },
        new[] {"GF","GUF","254","French Guiana", "The French Guiana Territorial Collectivity", "French Guiana" },
        new[] {"GG","GGY","831","Guernsey", "Guernsey", "Guernsey" },
        new[] {"GH","GHA","288","Ghana", "The Republic of Ghana", "Ghana" },
        new[] {"GI","GIB","292","Gibraltar", "Gibraltar", "Gibraltar" },
        new[] {"GL","GRL","304","Greenland", "Greenland", "Greenland" },
        new[] {"GM","GMB","270","Gambia (the)", "The Republic of the Gambia", "The Gambia" },
        new[] {"GN","GIN","324","Guinea", "The Republic of Guinea", "Guinea" },
        new[] {"GP","GLP","312","Guadeloupe", "The Republic of Guinea", "Guadeloupe" },
        new[] {"GQ","GNQ","226","Equatorial Guinea", "The Republic of Equatorial Guinea", "Equatorial Guinea" },
        new[] {"GR","GRC","300","Greece", "The Hellenic Republic", "Greece" },
        new[] {"GS","SGS","239","South Georgia and the South Sandwich Islands", "South Georgia and the South Sandwich Islands", "South Georgia and the South Sandwich Islands" },
        new[] {"GT","GTM","320","Guatemala", "The Republic of Guatemala", "Guatemala" },
        new[] {"GU","GUM","316","Guam", "Guam", "Guam" },
        new[] {"GW","GNB","624","Guinea-Bissau", "The Republic of Guinea-Bissau", "Guinea-Bissau" },
        new[] {"GY","GUY","328","Guyana", "The Co-operative Republic of Guyana", "Guyana" },
        new[] {"HK","HKG","344","Hong Kong", "The Hong Kong Special Administrative Region of the People's Republic of China", "Hong Kong" },
        new[] {"HM","HMD","334","Heard Island and McDonald Islands", "The Territory of Heard Island and McDonald Islands", "Heard Island and McDonald Islands" },
        new[] {"HN","HND","340","Honduras", "The Republic of Honduras", "Honduras" },
        new[] {"HR","HRV","191","Croatia", "The Republic of Croatia", "Croatia" },
        new[] {"HT","HTI","332","Haiti", "The Republic of Haiti", "Haiti" },
        new[] {"HU","HUN","348","Hungary", "Hungary", "Hungary" },
        new[] {"ID","IDN","360","Indonesia", "The Republic of Indonesia", "Indonesia" },
        new[] {"IE","IRL","372","Ireland", "Ireland", "Ireland" },
        new[] {"IL","ISR","376","Israel", "The State of Israel", "Israel" },
        new[] {"IM","IMN","833","Isle of Man", "The Isle of Man", "Isle of Man" },
        new[] {"IN","IND","356","India", "The Republic of India", "India" },
        new[] {"IO","IOT","086","British Indian Ocean Territory (the)", "The British Indian Ocean Territory", "British Indian Ocean Territory" },
        new[] {"IQ","IRQ","368","Iraq", "The Republic of Iraq", "Iraq" },
        new[] {"IR","IRN","364","Iran (Islamic Republic of)", "The Islamic Republic of Iran", "Iran" },
        new[] {"IS","ISL","352","Iceland", "The Republic of Iceland", "Iceland" },
        new[] {"IT","ITA","380","Italy", "The Republic of Italy", "Italy" },
        new[] {"JE","JEY","832","Jersey", "The Bailiwick of Jersey", "Jersey" },
        new[] {"JM","JAM","388","Jamaica", "Jamaica", "Jamaica" },
        new[] {"JO","JOR","400","Jordan", "The Hashemite Kingdom of Jordan", "Jordan" },
        new[] {"JP","JPN","392","Japan", "Japan", "Japan" },
        new[] {"KE","KEN","404","Kenya", "The Republic of Kenya", "Kenya" },
        new[] {"KG","KGZ","417","Kyrgyzstan", "The Kyrgyz Republic", "Kyrgyzstan" },
        new[] {"KH","KHM","116","Cambodia", "The Kingdom of Cambodia", "Cambodia" },
        new[] {"KI","KIR","296","Kiribati", "The Republic of Kiribati", "Kiribati" },
        new[] {"KM","COM","174","Comoros (the)", "The Union of the Comoros", "Comoros" },
        new[] {"KN","KNA","659","Saint Kitts and Nevis", "The Federation of Saint Christopher and Nevis", "Saint Kitts and Nevis" },
        new[] {"KP","PRK","408","Korea (the Democratic People's Republic of)", "The Democratic People's Republic of Korea", "North Korea" },
        new[] {"KR","KOR","410","Korea (the Republic of)", "The Republic of Korea", "South Korea" },
        new[] {"KW","KWT","414","Kuwait", "The State of Kuwait", "Kuwait" },
        new[] {"KY","CYM","136","Cayman Islands (the)", "The Cayman Islands", "Cayman Islands" },
        new[] {"KZ","KAZ","398","Kazakhstan", "The Republic of Kazakhstan", "Kazakhstan" },
        new[] {"LA","LAO","418","Lao People's Democratic Republic (the)", "The Lao People's Democratic Republic", "Laos" },
        new[] {"LB","LBN","422","Lebanon", "The Lebanese Republic", "Lebanon" },
        new[] {"LC","LCA","662","Saint Lucia", "Saint Lucia", "Saint Lucia" },
        new[] {"LI","LIE","438","Liechtenstein", "The Principality of Liechtenstein", "Liechtenstein" },
        new[] {"LK","LKA","144","Sri Lanka", "The Democratic Socialist Republic of Sri Lanka", "Sri Lanka" },
        new[] {"LR","LBR","430","Liberia", "The Republic of Liberia", "Liberia" },
        new[] {"LS","LSO","426","Lesotho", "The Kingdom of Lesotho", "Lesotho" },
        new[] {"LT","LTU","440","Lithuania", "The Republic of Lithuania", "Lithuania" },
        new[] {"LU","LUX","442","Luxembourg", "The Grand Duchy of Luxembourg", "Luxembourg" },
        new[] {"LV","LVA","428","Latvia", "The Republic of Latvia", "Latvia" },
        new[] {"LY","LBY","434","Libya", "The State of Libya", "Libya"},
        new[] {"MA","MAR","504","Morocco", "The Kingdom of Morocco", "Morocco" },
        new[] {"MC","MCO","492","Monaco", "The Principality of Monaco", "Monaco" },
        new[] {"MD","MDA","498","Moldova (the Republic of)", "The Republic of Moldova", "Moldova" },
        new[] {"ME","MNE","499","Montenegro", "Montenegro", "Montenegro" },
        new[] {"MF","MAF","663","Saint Martin (French part)", "Saint Martin", "Saint Martin" },
        new[] {"MG","MDG","450","Madagascar", "The Republic of Madagascar", "Madagascar" },
        new[] {"MH","MHL","584","Marshall Islands (the)", "The Republic of the Marshall Islands", "Marshall Islands" },
        new[] {"MK","MKD","807","North Macedonia", "The Republic of North Macedonia", "North Macedonia" },
        new[] {"ML","MLI","466","Mali", "The Republic of Mali", "Mali" },
        new[] {"MM","MMR","104","Myanmar", "The Republic of the Union of Myanmar", "Myanmar" },
        new[] {"MN","MNG","496","Mongolia", "Mongolia", "Mongolia" },
        new[] {"MO","MAC","446","Macao", "Macao Special Administrative Region of the People's Republic of China", "Macao" },
        new[] {"MP","MNP","580","Northern Mariana Islands (the)", "The Commonwealth of the Northern Mariana Islands", "Northern Mariana Islands" },
        new[] {"MQ","MTQ","474","Martinique", "Martinique", "Martinique" },
        new[] {"MR","MRT","478","Mauritania", "The Islamic Republic of Mauritania", "Mauritania" },
        new[] {"MS","MSR","500","Montserrat", "Montserrat", "Montserrat" },
        new[] {"MT","MLT","470","Malta", "The Republic of Malta", "Malta" },
        new[] {"MU","MUS","480","Mauritius", "The Republic of Mauritius", "Mauritius" },
        new[] {"MV","MDV","462","Maldives", "The Republic of Maldives", "Maldives" },
        new[] {"MW","MWI","454","Malawi", "The Republic of Malawi", "Malawi" },
        new[] {"MX","MEX","484","Mexico", "The United Mexican States", "Mexico" },
        new[] {"MY","MYS","458","Malaysia", "Malaysia", "Malaysia" },
        new[] {"MZ","MOZ","508","Mozambique", "The Republic of Mozambique", "Mozambique" },
        new[] {"NA","NAM","516","Namibia", "The Republic of Namibia", "Namibia" },
        new[] {"NC","NCL","540","New Caledonia", "New Caledonia", "New Caledonia" },
        new[] {"NE","NER","562","Niger (the)", "The Republic of the Niger", "Niger" },
        new[] {"NF","NFK","574","Norfolk Island", "The Territory of Norfolk Island", "Norfolk Island" },
        new[] {"NG","NGA","566","Nigeria", "The Federal Republic of Nigeria", "Nigeria" },
        new[] {"NI","NIC","558","Nicaragua", "The Republic of Nicaragua", "Nicaragua" },
        new[] {"NL","NLD","528","Netherlands (the)", "The Kingdom of the Netherlands", "Netherlands" },
        new[] {"NO","NOR","578","Norway", "The Kingdom of Norway", "Norway" },
        new[] {"NP","NPL","524","Nepal", "The Federal Democratic Republic of Nepal", "Nepal" },
        new[] {"NR","NRU","520","Nauru", "The Republic of Nauru", "Nauru" },
        new[] {"NU","NIU","570","Niue", "Niue", "Niue" },
        new[] {"NZ","NZL","554","New Zealand", "New Zealand", "New Zealand" },
        new[] {"OM","OMN","512","Oman", "The Sultanate of Oman", "Oman" },
        new[] {"PA","PAN","591","Panama", "The Republic of Panama", "Panama" },
        new[] {"PE","PER","604","Peru", "The Republic of Peru", "Peru" },
        new[] {"PF","PYF","258","French Polynesia", "French Polynesia", "French Polynesia" },
        new[] {"PG","PNG","598","Papua New Guinea", "The Independent State of Papua New Guinea", "Papua New Guinea" },
        new[] {"PH","PHL","608","Philippines (the)", "The Republic of the Philippines", "Philippines" },
        new[] {"PK","PAK","586","Pakistan", "The Islamic Republic of Pakistan", "Pakistan" },
        new[] {"PL","POL","616","Poland", "The Republic of Poland", "Poland" },
        new[] {"PM","SPM","666","Saint Pierre and Miquelon", "The Territorial Collectivity of Saint-Pierre and Miquelon", "Saint Pierre and Miquelon" },
        new[] {"PN","PCN","612","Pitcairn", "The Pitcairn, Henderson, Ducie and Oeno Islands", "Pitcairn Islands" },
        new[] {"PR","PRI","630","Puerto Rico", "The Commonwealth of Puerto Rico", "Puerto Rico" },
        new[] {"PS","PSE","275","Palestine, State of", "The State of Palestine", "Palestine" },
        new[] {"PT","PRT","620","Portugal", "The Portuguese Republic","Portugal" },
        new[] {"PW","PLW","585","Palau", "The Republic of Palau","Palau" },
        new[] {"PY","PRY","600","Paraguay", "The Republic of Paraguay","Paraguay" },
        new[] {"QA","QAT","634","Qatar", "The State of Qatar","Qatar" },
        new[] {"RE","REU","638","Réunion", "Réunion", "Réunion" },
        new[] {"RO","ROU","642","Romania", "Romania", "Romania" },
        new[] {"RS","SRB","688","Serbia", "The Republic of Serbia","Serbia" },
        new[] {"RU","RUS","643","Russian Federation (the)", "The Russian Federation", "Russia" },
        new[] {"RW","RWA","646","Rwanda", "The Republic of Rwanda","Rwanda" },
        new[] {"SA","SAU","682","Saudi Arabia", "The Kingdom of Saudi Arabia","Saudi Arabia" },
        new[] {"SB","SLB","090","Solomon Islands", "Solomon Islands", "Solomon Islands" },
        new[] {"SC","SYC","690","Seychelles", "The Republic of Seychelles", "Seychelles" },
        new[] {"SD","SDN","729","Sudan (the)", "The Republic of the Sudan", "Sudan" },
        new[] {"SE","SWE","752","Sweden", "The Kingdom of Sweden","Sweden" },
        new[] {"SG","SGP","702","Singapore", "The Republic of Singapore","Singapore" },
        new[] {"SH","SHN","654","Saint Helena, Ascension and Tristan da Cunha", "Saint Helena, Ascension and Tristan da Cunha", "Saint Helena" },
        new[] {"SI","SVN","705","Slovenia", "The Republic of Slovenia","Slovenia" },
        new[] {"SJ","SJM","744","Svalbard and Jan Mayen", "Svalbard and Jan Mayen", "Svalbard and Jan Mayen" },
        new[] {"SK","SVK","703","Slovakia", "The Slovak Republic","Slovakia" },
        new[] {"SL","SLE","694","Sierra Leone", "The Republic of Sierra Leone", "Sierra Leone" },
        new[] {"SM","SMR","674","San Marino", "The Republic of San Marino","San Marino" },
        new[] {"SN","SEN","686","Senegal", "The Republic of Senegal","Senegal" },
        new[] {"SO","SOM","706","Somalia", "The Federal Republic of Somalia","Somalia" },
        new[] {"SR","SUR","740","Suriname", "The Republic of Suriname","Suriname" },
        new[] {"SS","SSD","728","South Sudan", "The Republic of South Sudan", "South Sudan" },
        new[] {"ST","STP","678","Sao Tome and Principe", "The Democratic Republic of Sao Tome and Principe", "São Tomé and Príncipe" },
        new[] {"SV","SLV","222","El Salvador", "The Republic of El Salvador", "El Salvador" },
        new[] {"SX","SXM","534","Sint Maarten (Dutch part)", "Sint Maarten", "Sint Maarten" },
        new[] {"SY","SYR","760","Syrian Arab Republic (the)", "The Syrian Arab Republic", "Syria" },
        new[] {"SZ","SWZ","748","Eswatini", "The Kingdom of Eswatini","Eswatini" },
        new[] {"TC","TCA","796","Turks and Caicos Islands (the)", "The Turks and Caicos Islands", "Turks and Caicos Islands" },
        new[] {"TD","TCD","148","Chad", "The Republic of Chad","Chad" },
        new[] {"TF","ATF","260","French Southern Territories (the)", "The French Southern and Antarctic Lands", "French Southern and Antarctic Lands" },
        new[] {"TG","TGO","768","Togo", "The Togolese Republic","Togo" },
        new[] {"TH","THA","764","Thailand", "The Kingdom of Thailand","Thailand" },
        new[] {"TJ","TJK","762","Tajikistan", "The Republic of Tajikistan","Tajikistan" },
        new[] {"TK","TKL","772","Tokelau", "The Tokelau Islands", "Tokelau" },
        new[] {"TL","TLS","626","Timor-Leste", "The Democratic Republic of Timor-Leste", "Timor-Leste" },
        new[] {"TM","TKM","795","Turkmenistan", "Turkmenistan", "Turkmenistan" },
        new[] {"TN","TUN","788","Tunisia", "The Republic of Tunisia","Tunisia" },
        new[] {"TO","TON","776","Tonga", "The Kingdom of Tonga","Tonga" },
        new[] {"TR","TUR","792","Turkey", "The Republic of Turkey","Turkey" },
        new[] {"TT","TTO","780","Trinidad and Tobago", "The Republic of Trinidad and Tobago", "Trinidad and Tobago" },
        new[] {"TV","TUV","798","Tuvalu", "Tuvalu", "Tuvalu" },
        new[] {"TW","TWN","158","Taiwan (Province of China)", "The Republic of China", "Taiwan" },
        new[] {"TZ","TZA","834","Tanzania, the United Republic of", "The United Republic of Tanzania", "Tanzania" },
        new[] {"UA","UKR","804","Ukraine", "Ukraine", "Ukraine" },
        new[] {"UG","UGA","800","Uganda", "The Republic of Uganda","Uganda" },
        new[] {"UM","UMI","581","United States Minor Outlying Islands (the)", "United States Minor Outlying Islands", "United States Minor Outlying Islands" },
        new[] {"US","USA","840","United States of America (the)", "The United States of America", "United States" },  // https://www.iso.org/obp/ui/#iso:code:3166:US
        new[] {"UY","URY","858","Uruguay", "The Eastern Republic of Uruguay","Uruguay" },
        new[] {"UZ","UZB","860","Uzbekistan", "The Republic of Uzbekistan","Uzbekistan" },
        new[] {"VA","VAT","336","Holy See (the)", "The Holy See", "Vatican City"},
        new[] {"VC","VCT","670","Saint Vincent and the Grenadines", "Saint Vincent and the Grenadines", "St. Vincent and the Grenadines" },
        new[] {"VE","VEN","862","Venezuela (Bolivarian Republic of)", "The Bolivarian Republic of Venezuela", "Venezuela" },
        new[] {"VG","VGB","092","Virgin Islands (British)", "The Virgin Islands", "British Virgin Islands" },
        new[] {"VI","VIR","850","Virgin Islands (U.S.)", "The Virgin Islands of the United States", "U.S. Virgin Islands" },
        new[] {"VN","VNM","704","Viet Nam", "The Socialist Republic of Vietnam", "Vietnam" },
        new[] {"VU","VUT","548","Vanuatu", "The Republic of Vanuatu", "Vanuatu" },
        new[] {"WF","WLF","876","Wallis and Futuna", "Wallis and Futuna Islands", "Wallis and Futuna" },
        new[] {"WS","WSM","882","Samoa", "The Independent State of Samoa", "Samoa" },
        new[] {"YE","YEM","887","Yemen", "The Republic of Yemen", "Yemen" },
        new[] {"YT","MYT","175","Mayotte", "The Department of Mayotte", "Mayotte" },
        new[] {"ZA","ZAF","710","South Africa", "The Republic of South Africa", "South Africa" },
        new[] {"ZM","ZMB","894","Zambia", "The Republic of Zambia", "Zambia" },
        new[] {"ZW","ZWE","716","Zimbabwe", "The Republic of Zimbabwe", "Zimbabwe" },
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