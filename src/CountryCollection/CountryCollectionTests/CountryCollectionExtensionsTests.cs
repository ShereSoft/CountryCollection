using System;
using System.Linq;
using Xunit;

namespace CountryCollectionTests
{
    public class CountryCollectionExtensionsTests
    {
		[Fact]
		void Test_GetCountryExtension_ValidCountry()
		{
			Assert.NotNull(new CountryCollection().GetCountry("US"));
		}

		[Fact]
		void Test_ContainsExtension_ValidCountry()
		{
			Assert.True(new CountryCollection().Contains("US"));
		}

		[Fact]
		void Test_NormalizeExtension_ValidCountry()
		{
			Assert.NotNull(new CountryCollection().Normalize("US"));
		}
	}
}
