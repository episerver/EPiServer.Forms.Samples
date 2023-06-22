using EPiServer.ServiceLocation;
using System.Collections.Generic;

namespace EPiServer.Forms.Samples.Configuration
{
    [Options(ConfigurationSection = "FormSamples")]
    public class FormSamplesApiKeyOptions
    {
        public AddressKey AddressKey { get; set; }
        public RecaptchaKey RecaptchaKey { get; set; }
        public HcaptchaKey HcaptchaKey { get; set; }
    }

    public class HcaptchaKey
    {
        public string SiteKey { get; set; }
        public string SecretKey { get; set; }
    }

    public class AddressKey
    {
        public string ClientApiKey { get;set;}
        public string ServerApiKey {get;set;}
    }

    public class RecaptchaKey
    {
        public string SiteKey {get;set;}
        public string SecretKey {get;set;}
    }
}
