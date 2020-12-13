using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeWebApp.Models.ViewModels
{
    public class MasterViewModel
    {
        public Country FromCountry { get; set; }
        public Country ToCountry { get; set; }
        public CurrencyRate FromCountryRate { get; set; }
        public CurrencyRate ToCountryRate { get; set; }
        public CurrencyRate CurrencyRate { get; set; }
        public int CountryId { get; set; }
        public int CurrencyRateId { get; set; }
        public int FromCountryId { get; set; }
        public int ToCountryId { get; set; }
        public decimal Amount { get; set; }
        public decimal ConversionResult { get; set; }
        public DateTime SpecifiedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<Country> CountryList { get; set; }
        public IEnumerable<CurrencyRate> CurrencyRateList { get; set; }
        public IEnumerable<CurrencyRate> FromCurrencyRateList { get; set; }
        public IEnumerable<CurrencyRate> ToCurrencyRateList { get; set; }
    }
}
