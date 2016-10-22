using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;
using System.Web.Http;
using System.Web.Script.Serialization;
using ZebPayCurrencyConverter.Common;

namespace ZebPayCurrencyConverter.Controllers
{
    public class v0Controller : ApiController
    {
        #region "Variables"
        CurrencyConverterEntities st = new CurrencyConverterEntities();
        JavaScriptSerializer jsContentDetails;
        dynamic dataList,totalAmount = 0.0m, conversionRateExist = 0.0M;
        SendJsonResponse jsonCurrConvertResponse;
        CurrencyInfo getInfo;
        Utility utl = new Utility();
        #endregion

        #region "methods"

        [System.Web.Http.HttpPost]
        public IEnumerable rate(JObject currencyContent)
        {
            dataList = currencyContent;
            jsContentDetails = new JavaScriptSerializer();
            try
            {
                getInfo = jsContentDetails.Deserialize<CurrencyInfo>(currencyContent.ToString());
                jsonCurrConvertResponse = new SendJsonResponse();
                // Have done ToUpper() if user enters data outside the website....(E.g. from https://www.hurl.it/).
                if (getInfo.currencyAmount != 0.0m && getInfo.fromCurrencyCode.ToUpper() != null)
                {
                    conversionRateExist = CheckRate(getInfo.fromCurrencyCode.ToUpper());
                    if (conversionRateExist != 0.0M)
                    {
                        totalAmount = CalculateAmount(getInfo.currencyAmount, conversionRateExist);
                        jsonCurrConvertResponse.SourceCurrency = getInfo.fromCurrencyCode.ToUpper();
                        jsonCurrConvertResponse.ConversionRate = conversionRateExist;
                        jsonCurrConvertResponse.Amount = getInfo.currencyAmount;
                        jsonCurrConvertResponse.Total = totalAmount;
                        jsonCurrConvertResponse.returncode = 1;
                        jsonCurrConvertResponse.err = "success";
                        jsonCurrConvertResponse.timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    }
                }
                return jsContentDetails.Serialize(jsonCurrConvertResponse);
            }
            catch (Exception ex)
            {   
                jsonCurrConvertResponse.SourceCurrency = getInfo.fromCurrencyCode;
                jsonCurrConvertResponse.ConversionRate = conversionRateExist;
                jsonCurrConvertResponse.Amount = getInfo.currencyAmount;
                jsonCurrConvertResponse.Total = totalAmount;
                jsonCurrConvertResponse.returncode = 0;
                jsonCurrConvertResponse.err = "failure";
                jsonCurrConvertResponse.timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                utl.LogWebError("v0", "rate", ex.Message);
                return jsContentDetails.Serialize(jsonCurrConvertResponse);
            }
        }

        /// <summary>
        /// CheckRate
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="toCurrency"></param>
        /// <returns></returns>
        public decimal CheckRate(string toCurrency)
        {
            try
            {
                var todaysDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                var currency = st.ssz_exchange_rate.OrderByDescending(x => x.exchange_date).Where(x => x.ssz_country.zcountry_currency_code == toCurrency).FirstOrDefault().exchange_rate_amount;
                if (currency != null)
                    return Math.Round(Convert.ToDecimal(currency), 2);
                else
                    return 0.0M;
            }
            catch (Exception ex)
            { utl.LogWebError("v0", "rate", ex.Message); return 0.0M; }
        }

        /// <summary>
        /// CalculateAmount
        /// </summary>
        /// <param name="sourceAmount"></param>
        /// <param name="sourceRate"></param>
        /// <returns></returns>
        public decimal CalculateAmount(decimal sourceAmount, decimal sourceRate)
        {
            try
            {
                if (sourceAmount != 0.0m && sourceRate != 0.0m)
                    return Math.Round((sourceAmount * sourceRate), 2);
                else
                    return 0.0m;
            }
            catch (Exception ex)
            {
                utl.LogWebError("v0", "rate", ex.Message);
                return 0.0m;
            }
        }

        #endregion
    }

    #region "CurrencyInfo"
    public class CurrencyInfo
    {
        public string fromCurrencyCode { get; set; }
        public decimal currencyAmount { get; set; }
    }
    #endregion

    #region "SendJsonResponse"
    
    public class SendJsonResponse
    {
        public string SourceCurrency { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal Amount { get; set; }
        public decimal Total { get; set; }
        public Int16 returncode { get; set; }
        public string err { get; set; }
        public string timestamp { get; set; }
    }
    #endregion
}
