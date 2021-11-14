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
