using System.Net;
using log4net;
using Sage.SData.Client.Core;

namespace EmailMarketing.SalesLogix
{
    internal static class SDataClientExceptionLogger
    {
        //This method attempts to interpret the exception passed, and if possible logs an informative error.
        public static LoggingAction OutputToLog(SDataClientException ex, string sdataUrl, ILog logger)
        {
            if (string.IsNullOrWhiteSpace(sdataUrl))
                sdataUrl = ".";
            else
                sdataUrl = " at (" + sdataUrl + ")";

            if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException is WebException)
            {
                WebException webEx = (WebException)ex.InnerException.InnerException;
                if (webEx.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    logger.ErrorFormat("Name resolution failure when connecting to SalesLogix SalesLogix system{0}", sdataUrl);
                    return LoggingAction.ErrorLogged;
                }

                HttpWebResponse webResp = webEx.Response as HttpWebResponse;
                if (webResp != null)
                {
                    if (webResp.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        logger.ErrorFormat("The credentials given for connecting to SalesLogix are incorrect.  Not authorized to connect to SalesLogix system{0}", sdataUrl);
                        return LoggingAction.ErrorLogged;
                    }

                    if (webResp.StatusCode == HttpStatusCode.NotFound)
                    {
                        logger.ErrorFormat("(404) Not Found error.  Could not connect to SalesLogix system{0}", sdataUrl);
                        return LoggingAction.ErrorLogged;
                    }
                }
            }

            return LoggingAction.NoErrorLogged;
        }
    }

    public enum LoggingAction
    {
        ErrorLogged,
        NoErrorLogged
    };
}