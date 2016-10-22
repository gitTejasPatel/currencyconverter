using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZebPayCurrencyConverter.Common
{
    public class Utility
    {
        #region "Variables"
        CurrencyConverterEntities st = new CurrencyConverterEntities();
        #endregion

        public void LogWebError(string controllerName, string actionName, string errorMessage)
        {
            try
            {
                ssz_error_log errorLog = new ssz_error_log();
                errorLog.controller_name = controllerName;
                errorLog.action_Name = actionName;
                errorLog.error_msg = errorMessage;
                errorLog.date_of_occurence = DateTime.Now;
                st.ssz_error_log.Add(errorLog);
                st.SaveChanges();
            }
            catch (Exception ex)
            {
                LogWebError("Utility", "LogWebError", ex.Message);
            }
        }
    }
}