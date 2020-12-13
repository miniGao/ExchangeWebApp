using CurrencyExchangeWebApp.Models;
using CurrencyExchangeWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchangeWebApp.Controllers
{
    public class HomeController : Controller
    {
        private HttpClientWorker worker = new HttpClientWorker();
        private HttpClient client;
        private string json;
        private HttpResponseMessage response;

        public HomeController()
        {
            // instantiate httpclient
            client = worker.client;
        }

        /*
         1 method in Currency Conversion:
        1. Convert one currency to another, using USD as the base currency. (include a combination of Get methods, and some calculation logics)

         8 methods in Currency Management: 
        1. GET: currencyapi/currencyrate/country/{countryId}
        2. GET currencyapi/currencyrate/country/{countryId}/latest
        3. GET currencyapi/currencyrate/country/{countryId}/ondate/{date}
        4. GET currencyapi/currencyrate/country/{countryId}/datebetween/{startdate}to{enddate}
        5. GET currencyapi/currencyrate/country/{countryId}/getrate/{rateid}
        6. POST currencyapi/currencyrate/country/{countryId}/addrate
        7. PUT currencyapi/currencyrate/country/{countryId}/updaterate/{rateid}
        8. DELETE currencyapi/currencyrate/country/{countryId}/deleterate/{rateid}

         5 methods in Country Management:
        1. GET: currencyapi/countrylist.json
        2. GET currencyapi/countryinfo/{countryId}(?isCurrencyRateIncluded=true)
        3. POST currencyapi/addcountry
        4. PUT currencyapi/updatecountry/{countryId}
        5. DELETE currencyapi/deletecountry/{countryId}
         */

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            MasterViewModel vm = new MasterViewModel();
            try
            {
                // [Country Management] 1. GET: currencyapi/countrylist.json
                response = await client.GetAsync("gaoli_ducdang_api/currencyapi/countrylist.json");
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                    IEnumerable<Country> countries = JsonConvert.DeserializeObject<IEnumerable<Country>>(json);
                    vm.CountryList = countries;
                    return View(vm);
                }
                TempData["warning"] = "Internal Server Error";
                return View(new MasterViewModel { CountryList = new List<Country>() });
            } catch (Exception e)
            {
                TempData["warning"] = e.Message;
                return View(new MasterViewModel { CountryList = new List<Country>() });
            }
        }

        [HttpGet]
        public IActionResult AddCountry()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCountryAsync(Country country)
        {
            try
            {
                Country countryToAdd = new Country { CountryName = country.CountryName, CurrencyCode = country.CurrencyCode };
                json = JsonConvert.SerializeObject(countryToAdd);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                // [Country Management] 3. POST currencyapi/addcountry
                response = await client.PostAsync("gaoli_ducdang_api/currencyapi/addcountry", content);
                response.EnsureSuccessStatusCode();
                json = await response.Content.ReadAsStringAsync();
                TempData["info"] = $"POST Status: {response.StatusCode}\nCountry Added (JSON): {json}";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["warning"] = e.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<IActionResult> UpdateCountryAsync(int countryId)
        {
            Country country = new Country();
            country.CountryId = countryId;
            try
            {
                // [Country Management] 2. GET currencyapi/countryinfo/{countryId}
                response = await client.GetAsync($"gaoli_ducdang_api/currencyapi/countryinfo/{countryId}");
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                    Country existingCountry = JsonConvert.DeserializeObject<Country>(json);
                    country.CountryName = existingCountry.CountryName;
                    country.CurrencyCode = existingCountry.CurrencyCode;
                    return View(country);
                }
                else
                {
                    TempData["warning"] = "Internal Server Error";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                TempData["warning"] = e.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCountryAsync(Country updateCountry)
        {
            try
            {
                Country countryToUpdate = new Country { CountryName = updateCountry.CountryName, CurrencyCode = updateCountry.CurrencyCode };
                json = JsonConvert.SerializeObject(countryToUpdate);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                // [Country Management] 4. PUT currencyapi/updatecountry/{countryId}
                response = await client.PutAsync($"gaoli_ducdang_api/currencyapi/updatecountry/{updateCountry.CountryId}", content);
                response.EnsureSuccessStatusCode();
                TempData["info"] = $"PUT Status: {response.StatusCode}";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["warning"] = e.Message;
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> DeleteCountryAsync(int countryId)
        {
            try
            {
                // [Country Management] 5. DELETE currencyapi/deletecountry/{countryId}
                response = await client.DeleteAsync($"gaoli_ducdang_api/currencyapi/deletecountry/{countryId}");
                response.EnsureSuccessStatusCode();
                TempData["info"] = $"DELETE Status: {response.StatusCode}";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["warning"] = e.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CurrencyRateHistory(int countryId)
        {
            MasterViewModel vm = new MasterViewModel();
            vm.CountryId = countryId;
            try
            {
                // [Currency Management] 1. GET: currencyapi/currencyrate/country/{countryId}
                response = await client.GetAsync($"gaoli_ducdang_api/currencyapi/currencyrate/country/{countryId}");
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                    vm.CurrencyRateList = JsonConvert.DeserializeObject<IEnumerable<CurrencyRate>>(json);
                    return View(vm);
                }
                TempData["warning"] = "Internal Server Error";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["warning"] = e.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConvertAsync(MasterViewModel vm)
        {
            decimal toCountryRate, fromCountryRate;
            try
            {
                // [Currency Management] 2. GET currencyapi/currencyrate/country/{countryId}/latest
                response = await client.GetAsync($"gaoli_ducdang_api/currencyapi/currencyrate/country/{vm.FromCountryId}/latest");
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                    vm.FromCountryRate = JsonConvert.DeserializeObject<CurrencyRate>(json);
                    fromCountryRate = vm.FromCountryRate.Rate;
                }
                else
                {
                    TempData["warning"] = "Internal Server Error";
                    return RedirectToAction("Index");
                }

                // [Currency Management] 2. GET currencyapi/currencyrate/country/{countryId}/latest
                response = await client.GetAsync($"gaoli_ducdang_api/currencyapi/currencyrate/country/{vm.ToCountryId}/latest");
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                    vm.ToCountryRate = JsonConvert.DeserializeObject<CurrencyRate>(json);
                    toCountryRate = vm.ToCountryRate.Rate;
                }
                else
                {
                    TempData["warning"] = "Internal Server Error";
                    return RedirectToAction("Index");
                }

                // [Country Management] 2. GET currencyapi/countryinfo/{countryId}?isCurrencyRateIncluded=true
                response = await client.GetAsync($"gaoli_ducdang_api/currencyapi/countryinfo/{vm.FromCountryId}?isCurrencyRateIncluded=true");
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                    vm.FromCountry = JsonConvert.DeserializeObject<Country>(json);
                    vm.FromCurrencyRateList = vm.FromCountry.CurrencyRate;
                }
                else
                {
                    TempData["warning"] = "Internal Server Error";
                    return RedirectToAction("Index");
                }
                // [Country Management] 2. GET currencyapi/countryinfo/{countryId}?isCurrencyRateIncluded=true
                response = await client.GetAsync($"gaoli_ducdang_api/currencyapi/countryinfo/{vm.ToCountryId}?isCurrencyRateIncluded=true");
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                    vm.ToCountry = JsonConvert.DeserializeObject<Country>(json);
                    vm.ToCurrencyRateList = vm.ToCountry.CurrencyRate;
                }
                else
                {
                    TempData["warning"] = "Internal Server Error";
                    return RedirectToAction("Index");
                }
                // calculate exchange logic
                if (toCountryRate != 0)
                {
                    vm.ConversionResult = (fromCountryRate / toCountryRate) * vm.Amount;
                }
                return View("ConversionResult", vm);
            }
            catch (Exception e)
            {
                TempData["warning"] = e.Message;
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> FilterByDateAsync(MasterViewModel vm)
        {
            List<CurrencyRate> currencyRates = new List<CurrencyRate>();
            try
            {
                // [Currency Management] 3. GET currencyapi/currencyrate/country/{countryId}/ondate/{date}
                response = await client.GetAsync($"gaoli_ducdang_api/currencyapi/currencyrate/country/{vm.CountryId}/ondate/{vm.SpecifiedDate}");
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                    vm.CurrencyRate = JsonConvert.DeserializeObject<CurrencyRate>(json);
                    currencyRates.Add(vm.CurrencyRate);
                    vm.CurrencyRateList = currencyRates;
                    return View("CurrencyRateHistory", vm);
                }
                else
                {
                    TempData["warning"] = "Internal Server Error";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                TempData["warning"] = e.Message;
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> FitlerBetweenDateAsync(MasterViewModel vm)
        {
            try
            {
                // [Currency Management] 4. GET currencyapi/currencyrate/country/{countryId}/datebetween/{startdate}to{enddate}
                response = await client.GetAsync($"gaoli_ducdang_api/currencyapi/currencyrate/country/{vm.CountryId}/datebetween/{vm.StartDate}to{vm.EndDate}");
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                    IEnumerable<CurrencyRate> currencyRates = JsonConvert.DeserializeObject<IEnumerable<CurrencyRate>>(json);
                    vm.CurrencyRateList = currencyRates;
                    return View("CurrencyRateHistory", vm);
                }
                else
                {
                    TempData["warning"] = "Internal Server Error";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                TempData["warning"] = e.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult AddCurrencyRate(int countryId)
        {
            MasterViewModel vm = new MasterViewModel();
            vm.CountryId = countryId;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddCurrencyRateAsync(MasterViewModel vm)
        {
            DateTime dateTime = vm.CurrencyRate.CurrencyDate;
            decimal rate = vm.CurrencyRate.Rate;
            try
            {
                CurrencyRate rateToAdd = new CurrencyRate { CurrencyDate = dateTime, Rate = rate };
                json = JsonConvert.SerializeObject(rateToAdd);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                // [Currency Management] 6. POST currencyapi/currencyrate/country/{countryId}/addrate
                response = await client.PostAsync($"gaoli_ducdang_api/currencyapi/currencyrate/country/{vm.CountryId}/addrate", content);
                response.EnsureSuccessStatusCode();
                json = await response.Content.ReadAsStringAsync();
                TempData["info"] = $"Currency Rate Added (JSON): {json}";
                return RedirectToAction("CurrencyRateHistory", new { countryId = vm.CountryId });
            }
            catch (Exception e)
            {
                TempData["warning"] = e.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCurrencyRateAsync(int countryId, int rateId)
        {
            MasterViewModel vm = new MasterViewModel();
            vm.CountryId = countryId;
            vm.CurrencyRateId = rateId;
            try
            {
                // [Currency Management] 5. GET currencyapi/currencyrate/country/{countryId}/getrate/{rateid}
                response = await client.GetAsync($"gaoli_ducdang_api/currencyapi/currencyrate/country/{countryId}/getrate/{rateId}");
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                    vm.CurrencyRate = JsonConvert.DeserializeObject<CurrencyRate>(json);
                    return View(vm);
                }
                else
                {
                    TempData["warning"] = $"Internal Server Error";
                    return RedirectToAction("CurrencyRateHistory", new { countryId = countryId });
                }
            }
            catch (Exception e)
            {
                TempData["warning"] = e.Message;
                return RedirectToAction("CurrencyRateHistory", new { countryId = countryId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCurrencyRate(MasterViewModel vm)
        {
            DateTime dateTime = vm.CurrencyRate.CurrencyDate;
            decimal rate = vm.CurrencyRate.Rate;
            try
            {
                CurrencyRate rateToUpdate = new CurrencyRate { CurrencyDate = dateTime, Rate = rate };
                json = JsonConvert.SerializeObject(rateToUpdate);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                // [Currency Management] 7. PUT currencyapi/currencyrate/country/{countryId}/updaterate/{rateid}
                response = await client.PutAsync($"gaoli_ducdang_api/currencyapi/currencyrate/country/{vm.CountryId}/updaterate/{vm.CurrencyRateId}", content);
                response.EnsureSuccessStatusCode();
                TempData["info"] = $"PUT Status: {response.StatusCode}";
                return RedirectToAction("CurrencyRateHistory", new { countryId = vm.CountryId });
            }
            catch (Exception e)
            {
                TempData["warning"] = e.Message;
                return RedirectToAction("UpdateCurrencyRate", new { countryId = vm.CountryId, rateId = vm.CurrencyRateId });
            }
        }

        public async Task<IActionResult> DeleteCurrencyRateAsync(int countryId, int rateId)
        {
            try
            {
                // [Currency Management] 8. DELETE currencyapi/currencyrate/country/{countryId}/deleterate/{rateid}
                response = await client.DeleteAsync($"gaoli_ducdang_api/currencyapi/currencyrate/country/{countryId}/deleterate/{rateId}");
                response.EnsureSuccessStatusCode();
                TempData["info"] = $"DELETE Status: {response.StatusCode}";
                return RedirectToAction("CurrencyRateHistory", new { countryId = countryId });
            }
            catch (Exception e)
            {
                TempData["warning"] = e.Message;
                return RedirectToAction("CurrencyRateHistory", new { countryId = countryId });
            }
        }
    }
}
