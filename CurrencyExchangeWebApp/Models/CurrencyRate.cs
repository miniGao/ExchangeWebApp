using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeWebApp.Models
{
    public class CurrencyRate
    {
        public int CurrencyRateId { get; set; }
        public DateTime CurrencyDate { get; set; }
        public decimal Rate { get; set; }
    }
}
