using EPiServer.Forms.Helpers.Internal;
using EPiServer.Forms.Samples.Business;
using EPiServer.Forms.Samples.Configuration;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EPiServer.Forms.Samples.Implementation.Validation
{
    [ServiceConfiguration(ServiceType = typeof(IAddressValidateService))]
    public class GoogleAddressValidateService : IAddressValidateService
    {
        private Injected<FormSamplesApiKeyOptions> _config;
        private readonly IHttpClientFactory _clientFactory;
        

        public GoogleAddressValidateService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            
        }

        public bool Validate(string address, string street, string city, string state, string postalCode, string country ,bool ignoreDetails = false)
        {

            var googleServerApiKey = _config.Service.AddressKey.ServerApiKey;
            var googleMapsGeocodingUrl = FormSamplesGoogleUrl.GoogleMapsGeocodingUrl;
            var currentPageLanguage = FormsExtensions.GetCurrentPageLanguage() ?? "en";

            if (string.IsNullOrWhiteSpace(googleServerApiKey))
            {
                return false;
            }

            var verifyUrl = googleMapsGeocodingUrl;

            if (!ignoreDetails && !string.IsNullOrWhiteSpace(address))
            {
                verifyUrl = verifyUrl.AddQueryString("address", address);
            }

            // build components filter
            List<string> componentFilter = new List<string>();
            if (!string.IsNullOrWhiteSpace(street))
            {
                componentFilter.Add("route:" + street);
            }
            if (!string.IsNullOrWhiteSpace(city))
            {
                componentFilter.Add("locality:" + city);
            }
            if (!string.IsNullOrWhiteSpace(state))
            {
                componentFilter.Add("administrative_area:" + state);
            }
            if (!string.IsNullOrWhiteSpace(postalCode))
            {
                componentFilter.Add("postal_code:" + postalCode);
            }
            if (!string.IsNullOrWhiteSpace(country))
            {
                componentFilter.Add("country:" + country);
            }
            if (componentFilter.Count > 0)
            {
                verifyUrl = verifyUrl.AddQueryString("components", string.Join("|", componentFilter));
            }
            verifyUrl = verifyUrl.AddQueryString("key", googleServerApiKey);
            verifyUrl = verifyUrl.AddQueryString("language", currentPageLanguage);

            // delegate the validation for google place
            try
            {
                var client = _clientFactory.CreateClient();
                var responseString = client.GetStringAsync(verifyUrl).Result;
                var result = JsonSerializer.Deserialize<GooglePlaceValidateResponse>(responseString);
                return result.status == GeocodingResponse.OK;
            }
            catch
            {
                return false;
            }
        }
    }

    public class GooglePlaceValidateResponse
    {
        public string status { get; set; }
    }

    public class GeocodingResponse
    {
        public const string OK = "OK";
        public const string ZERO_RESULTS = "ZERO_RESULTS";
        public const string OVER_QUERY_LIMIT = "OVER_QUERY_LIMIT";
        public const string REQUEST_DENIED = "REQUEST_DENIED";
        public const string INVALID_REQUEST = "INVALID_REQUEST";
        public const string UNKNOWN_ERROR = "UNKNOWN_ERROR";
    }
}
