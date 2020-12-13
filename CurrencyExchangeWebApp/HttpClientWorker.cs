using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CurrencyExchangeWebApp
{
    public class HttpClientWorker
    {
        public HttpClient client { get; set; }
        public HttpClientWorker()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://mrtrinh5293-eval-prod.apigee.net/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("apikey", "4ki43oZP9fJdV3rOfB76GO16rQW1iEWz");
        }
    }
}
