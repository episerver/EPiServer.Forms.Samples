using EPiServer.Forms.Core.Validation;
using EPiServer.Forms.Helpers.Internal;
using EPiServer.Forms.Implementation.Validation;
using EPiServer.Forms.Samples.Implementation.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EPiServer.Forms.Samples.Implementation.Validation
{
    public class HcaptchaValidator : InternalElementValidatorBase
    {
        private const string RecaptchaVerifyBaseUrl = "https://api.hcaptcha.com/siteverify";

        /// <inheritdoc />
        public override bool? Validate(IElementValidatable targetElement)
        {
            // NOTE: when run in none-js mode, the recaptcha element value will be null and validation failed.
            var submittedValue = targetElement.GetSubmittedValue().ToString();
            if (string.IsNullOrWhiteSpace(submittedValue))
            {
                return false;
            }

            var recaptchaElment = targetElement as HcaptchaElementBlock;
            if (recaptchaElment == null)
            {
                return false;
            }

            var client = new HttpClient();
            var verifyUrl = RecaptchaVerifyBaseUrl
                            .AddQueryString("secret", recaptchaElment.SecretKey)
                            .AddQueryString("response", submittedValue);
            var responseString = client.GetAsync(verifyUrl).Result;
            var result_json = responseString.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<HcaptchaResponse>(result_json);
            return result.success && result.score <= recaptchaElment.ScoreThreshold;
        }
    }

    public class HcaptchaResponse
    {
        /// <summary>
        /// Whether this request was a valid reCAPTCHA token for your site.
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// Timestamp of the challenge load (ISO format yyyy-MM-dd'T'HH:mm:ssZZ).
        /// </summary>
        public DateTime challenge_ts { get; set; }

        /// <summary>
        /// The hostname of the site where the reCAPTCHA was solved.
        /// </summary>
        public string hostname { get; set; }

        /// <summary>
        /// The score for this request (0.0 - 1.0).
        /// </summary>
        public double score { get; set; }
    }
}
