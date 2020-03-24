using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CLS_SLE.Utility
{
    public class SLEConfigurationInfo
    {
        public SSOConfigurationInfo SSOConfig
        {
            get => new SSOConfigurationInfo();
        }
    }

    public class SSOConfigurationInfo
    {

        public string Certificate
        {
            get => $"-----BEGIN CERTIFICATE-----\n{ConfigurationManager.AppSettings["SsoCertificate"]}\n-----END CERTIFICATE-----";
        }

        public string LoginURL
        {
            get => ConfigurationManager.AppSettings["SsoLoginURL"];
        }

        public string LogoutURL
        {
            get => ConfigurationManager.AppSettings["SsoLogoutURL"];
        }

        public string AssertionConsumerServiceURL
        {
            get => ConfigurationManager.AppSettings["SsoAssertionConsumerServiceURL"];
        }

        public string IssuerApplicationName
        {
            get => ConfigurationManager.AppSettings["SsoIssuerApplicationName"];
        }
    }
}