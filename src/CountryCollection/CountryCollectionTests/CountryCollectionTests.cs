using System;
using System.Linq;
using Xunit;

namespace CountryCollectionTests
{
    public class CountryCollectionTests
    {
		[Theory]
		[InlineData("US")]
		[InlineData("USA")]
		[InlineData("840")]
		[InlineData("us")]
		[InlineData("usa")]
		[InlineData("20")]
		void TestContains_ValidCountry(string value)
		{
			Assert.True(CountryCollection.Contains(value));
		}

		[Theory]
		[InlineData("ZZ")]
		[InlineData("ZZZ")]
		[InlineData("000")]
		[InlineData("zz")]
		[InlineData("zzz")]
		[InlineData("00")]
		void TestContains_InvalidCountry(string value)
		{
			Assert.False(CountryCollection.Contains(value));
		}

		[Fact]
		void TestContains_ThrowsWithNullCode()
		{
			Assert.Throws<ArgumentNullException>(() => CountryCollection.Contains(null));
		}

		[Theory]
		[InlineData(840)]
		[InlineData(20)]
		void TestNumericContains_ValidCountry(int value)
		{
			Assert.True(CountryCollection.Contains(value));
		}

		[Theory]
		[InlineData(9999)]
		[InlineData(999)]
		[InlineData(99)]
		[InlineData(0)]
		void TestNumericContains_InalidCountry(int value)
		{
			Assert.False(CountryCollection.Contains(value));
		}

		[Theory]
		[InlineData("US")]
		[InlineData("USA")]
		[InlineData("840")]
		[InlineData("us")]
		[InlineData("usa")]
		[InlineData("20")]
		void TestNormalize_ValidCountry(string value)
		{
			Assert.NotNull(CountryCollection.Normalize(value));
		}

		[Theory]
		[InlineData("ZZ")]
		[InlineData("ZZZ")]
		[InlineData("000")]
		[InlineData("zz")]
		[InlineData("zzz")]
		[InlineData("00")]
		void TestNormalize_InalidCountry(string value)
		{
			Assert.Null(CountryCollection.Normalize(value));
		}

		[Fact]
		void TestNormalize_ThrowsWithNullCode()
		{
			Assert.Throws<ArgumentNullException>(() => CountryCollection.Normalize(null));
		}

		[Theory]
		[InlineData(840)]
		[InlineData(20)]
		void TestNumericNormalize_ValidCountry(int value)
		{
			Assert.NotNull(CountryCollection.Normalize(value));
		}

		[Theory]
		[InlineData(9999)]
		[InlineData(999)]
		[InlineData(99)]
		[InlineData(0)]
		void TestNumericNormalize_InvalidCountry(int value)
		{
			Assert.Null(CountryCollection.Normalize(value));
		}

		[Fact]
		void Test_Enumeration()
		{
			Assert.True(new CountryCollection().Any());
		}

		[Fact]
		void Test_ReadOnlyEnumeration()
		{
			Assert.True(CountryCollection.Values.Any());
		}

		[Fact]
		void Test_CanGetCountryByCode()
		{
			var countries = new CountryCollection();

			Assert.NotNull(countries["US"]);
		}

		[Fact]
		void Test_CanUpdateGeneralName()
		{
			var update = "America";
			var country = CountryCollection.GetCountry("US");

			country.Name = update;

			Assert.Equal(update, CountryCollection.GetCountry("US").Name);
		}

		[Fact]
		void Test_CanUpdateFullName()
		{
			var update = "U.S.A.";
			var country = CountryCollection.GetCountry("US");

			country.FullName = update;

			Assert.Equal(update, CountryCollection.GetCountry("US").FullName);
		}
	}
}
