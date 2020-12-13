using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeWebApp.Models
{
    public class Country
    {
        public ICollection<CurrencyRate> CurrencyRate { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string CurrencyCode { get; set; }

        public Country()
        {
            CurrencyRate = new HashSet<CurrencyRate>();
        }
    }
}
