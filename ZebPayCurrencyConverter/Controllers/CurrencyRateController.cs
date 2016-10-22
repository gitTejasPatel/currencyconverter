using RestSharp;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Web.Mvc;

namespace ZebPayCurrencyConverter.Controllers
{
    public class CurrencyRateController : Controller
    {
        #region "Variables"
        CurrencyConverterEntities st = new CurrencyConverterEntities();
        string apiUrl = ConfigurationManager.AppSettings["ApiUrlClient"];
        decimal latestCurrencyRate = 0.0M; 
        string googleUrl = ConfigurationManager.AppSettings["GoogleCurrencyConvertUrl"].ToString(), postData = "", toCurrency = "INR";
        Int32 amount = 1;
        #endregion
        // GET: CurrencyRate
        public ActionResult CurrencyRate()
        {
            
            var getDestinationCountry = st.ssz_country.Where(z => z.zcountry_id != 1).ToList();
            ViewBag.destinationCountry = getDestinationCountry;
            return View();
        }

        public ActionResult GetRate(string currencyCode, decimal amount)
        {
            try
            {    
                var client = new RestClient(apiUrl);
                var request = new RestRequest("api/v0/rate", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(new
                {
                    fromCurrencyCode = currencyCode,
                    currencyAmount = amount
                });

                //execute the request
                IRestResponse responseContent = client.Execute(request);
                return Json(responseContent.Content, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("#Error");
            }
        }

        #region Periodically
        /// <summary>
        /// GetConversionRateTimer
        /// </summary>
        public void GetConversionRateTimer()
        {
            Timer timer = new Timer((60000 * 60 * 4)); // every 4 hours 
            timer.Elapsed += GetConversionRate;
            timer.Start();
        }
        /// <summary>
        /// GetConversionRate
        /// </summary>
        /// <returns></returns>
        public void GetConversionRate(Object source, ElapsedEventArgs e)
        {
            decimal exchangeRate = 0.0m;
            try
            {
                var getCurrencyRate = st.ssz_country.Where(s => s.zcountry_id != 1).ToList();
                foreach (var currencyCode in getCurrencyRate)
                {
                    string googleCurConverter = googleUrl + amount + "&from=" + currencyCode.zcountry_currency_code.ToString() + "&to=" + toCurrency;
                    exchangeRate = HttpPostRequest(googleCurConverter);
                    InsertExchangeRate(Convert.ToByte(currencyCode.zcountry_id), exchangeRate);
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// HttpPostRequest
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private decimal HttpPostRequest(string url)
        {
            try
            {

                HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myHttpWebRequest.Method = "POST";

                byte[] data = Encoding.ASCII.GetBytes(postData);
                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
                myHttpWebRequest.ContentLength = data.Length;

                Stream requestStream = myHttpWebRequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                Stream responseStream = myHttpWebResponse.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);
                string[] pageContent = Regex.Split(myStreamReader.ReadToEnd(), "&nbsp;");
                int first = pageContent[1].IndexOf("<div id=currency_converter_result>");
                int last = pageContent[1].LastIndexOf("</span>");

                string strCurrencyRate = pageContent[1].Substring(first, last - first);
                string[] currencyValues = strCurrencyRate.Split(' ');

                string strCurrencyValues = currencyValues[5].Remove(0, 10);
                latestCurrencyRate = Math.Round(Convert.ToDecimal(strCurrencyValues), 2);

                myStreamReader.Close();
                responseStream.Close();
                myHttpWebResponse.Close();
                return latestCurrencyRate;
            }
            catch (Exception ex)
            {
                return 0.0m;
            }
        }

        /// <summary>
        /// InsertExchangeRate
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="exchangeRate"></param>
        public void InsertExchangeRate(byte countryId, decimal exchangeRate)
        {
            try
            {
                ssz_exchange_rate sse = new ssz_exchange_rate();
                {
                    sse.zcountry_id = countryId;
                    sse.exchange_rate_amount = exchangeRate;
                    sse.exchange_date = DateTime.Now;
                    st.ssz_exchange_rate.Add(sse);
                    st.SaveChanges();
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion
    }
}