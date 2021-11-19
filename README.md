# CountryCollection
Provides convenient access to country info including country codes and country names based on ISO3166. Supports customization includng adding/removing countries and renaming country names. A simple alternative to the machine-dependent RegionInfo data in the standard library. No library dependencies. No external calls.

[![](https://img.shields.io/nuget/v/CountryCollection.svg)](https://www.nuget.org/packages/CountryCollection/)
[![](https://img.shields.io/nuget/dt/CountryCollection)](https://www.nuget.org/packages/CountryCollection/)

* Light-weight
* Device-agnostic country data (Alternative to the .NET Framework RegionInfo data)
* Actively maintained (New features are added frequently.)
* No external library dependencies
* No external calls


### .Contains(code)
```csharp
string code = "US";
bool valid = CountryCollection.Contains(code);  // True
```

### .Contains(code)
```csharp
string lowerCased = "us";
bool valid = CountryCollection.Contains(lowerCased);  // True
```

### .Contains(code)
```csharp
int numericCode = 840;
bool valid = CountryCollection.Contains(numericCode);  // True
```

### .Normalize(code)
```csharp
string iso2DigitCode = "us";
string code = CountryCollection.Normalize(iso2DigitCode);  // "US"
```

### .Normalize(code)
```csharp
string iso3DigitCode = "usa";
string code = CountryCollection.Normalize(iso3DigitCode);  // "USA"
```

### .Normalize(code)
```csharp
int numericCode = 32;
string code = CountryCollection.Normalize(numericCode);  // "032" (Argentina)
```

### .Normalize(code)
```csharp
string invalidCode = "xyz";
string code = CountryCollection.Normalize(invalidCode);  // null
```

### .Normalize(code)
```csharp
int numericCode = 99999;
string code = CountryCollection.Normalize(numericCode);  // null
```

### .GetCountry(code)
```csharp
string code = "US";
var country = CountryCollection.GetCountry(code);  // { "IsoAlpha2Code":"US", "IsoAlpha3Code":"USA", "IsoNumeric":840, "IsoEnglishShortName":"United States of America (the)", "Name":"United States" }
```

### .GetCountry(code)
```csharp
string invalidCode = "XYZ";
var country = CountryCollection.GetCountry(code);  // null
```

### _instance_.[code]
```csharp
string code = "US";
var country = new CountryCollection[code];  // { "IsoAlpha2Code":"US", "IsoAlpha3Code":"USA", "IsoNumeric":840, "IsoEnglishShortName":"United States of America (the)", "Name":"United States" }
```

### _instance_.Add(isoAlpha2Code, isoAlpha3Code, isoNumeric, isoName)
```csharp
var countries = new CountryCollection();
countries.Add("ZZ", "ZZZ", 999, "New Country");
```

### _instance_.Remove(code)
```csharp
var code = "ZZ";
var countries = new CountryCollection();
countries.Remove(code);
```

### Update CountryInfo.Name and CountryInfo.FullName
```csharp
string code = "US";
var country = new CountryCollection[code];

country.Name = "UNITED STATES";
country.FullName = "UNITED STATES OF AMERICA";
```

