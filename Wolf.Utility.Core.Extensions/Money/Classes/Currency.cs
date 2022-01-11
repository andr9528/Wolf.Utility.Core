using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Extensions.Money.Enums;

namespace Wolf.Utility.Core.Extensions.Money.Classes
{
	namespace Useful.Money
	{
		// Thanks! - https://searchcode.com/codesearch/view/27836890/
		public class Currency
		{
			public CurrencyCodes CurrencyCode { get; private set; }
			public string Code { get; private set; }
			public NumberFormatInfo NumberFormat { get; private set; }
			public string Symbol { get; private set; }
			public string EnglishName { get; private set; }

			/// <summary>
			/// Constructs a currency object with a NumberFormatInfo.
			/// </summary>
			/// <param name="currencyCode"></param>
			public Currency(CurrencyCodes currencyCode)
			{
				CurrencyCode = currencyCode;
				Code = Enum.GetName(typeof(CurrencyCodes), CurrencyCode);
				var cultureInfo = CultureInfoFromCurrencyISO(Code);
				NumberFormat = cultureInfo.NumberFormat;
				var region = new RegionInfo(cultureInfo.LCID);
				Symbol = region.CurrencySymbol;
				EnglishName = region.CurrencyEnglishName;
			}

			public static Currency Get(CurrencyCodes currencyCode)
			{
				if (CurrencyDictionary.ContainsKey(currencyCode))
					return CurrencyDictionary[currencyCode];
				else
					return null;
			}

			public static bool Exists(CurrencyCodes currencyCode)
			{
				return CurrencyDictionary.ContainsKey(currencyCode);
			}

			private static CultureInfo CultureInfoFromCurrencyISO(string isoCode)
			{
				//CultureInfo cultureInfo = (from culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
				//  let region = new RegionInfo(culture.LCID)
				//  where String.Equals(region.ISOCurrencySymbol, isoCode, StringComparison.InvariantCultureIgnoreCase)
				//  select culture).First();
				//return cultureInfo;
				foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
				{
					RegionInfo ri = new RegionInfo(ci.LCID);
					if (ri.ISOCurrencySymbol == isoCode)
						return ci;
				}
				throw new Exception("Currency code " + isoCode + " is not supported by the current .Net Framework.");
			}

			private static Dictionary<CurrencyCodes, Currency> _currencyDictionary;
			private static Dictionary<CurrencyCodes, Currency> CurrencyDictionary
			{
				get
				{
					if (_currencyDictionary == null)
						_currencyDictionary = CreateCurrencyDictionary();
					return _currencyDictionary;
				}
			}
			private static Dictionary<CurrencyCodes, Currency> CreateCurrencyDictionary()
			{
				var result = new Dictionary<CurrencyCodes, Currency>();
				foreach (CurrencyCodes code in Enum.GetValues(typeof(CurrencyCodes)))
					result.Add(code, new Currency(code));
				return result;
			}
		}
	}
}
