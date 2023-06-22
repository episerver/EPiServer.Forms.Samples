using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Forms.EditView;
using EPiServer.Forms.Implementation.Elements.BaseClasses;
using EPiServer.Forms.Samples.Configuration;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Forms.Core.Internal;
using EPiServer.Forms.Core;
using EPiServer.Forms.Samples.Implementation.Validation;
using Microsoft.AspNetCore.Http;
using EPiServer.Forms.Helpers.Internal;

namespace EPiServer.Forms.Samples.Implementation.Elements
{
    /// Haptcha element using HCAPTCHA. This element requires js, in non-js mode the validation will be failed.
    /// For get site key and secret key go to: https://dashboard.hcaptcha.com/ and register your site.
    /// </summary>
    [ContentType(GUID = "{2D7E4A18-8F8B-4C98-9E81-D97524C62562}", GroupName = ConstantsFormsUI.FormElementGroup, Order = 2910)]
    public class HcaptchaElementBlock : ValidatableElementBlockBase, IExcludeInSubmission, IViewModeInvisibleElement, IElementRequireClientResources
    {
        private static readonly ILogger _logger = LogManager.GetLogger(typeof(HcaptchaElementBlock));
        private Injected<FormSamplesApiKeyOptions> _config;

        [Ignore]
        public override string Label
        {
            get { return base.Label; }
            set { base.Label = value; }
        }

        [Ignore]
        public override string Description
        {
            get { return base.Description; }
            set { base.Description = value; }
        }

        #region IElementValidateable implement

        /// <summary>
        /// Always use CaptchaValidator to validate this element
        /// <remarks>hide from EditView</remarks>
        /// </summary>
        [Display(GroupName = SystemTabNames.Content, Order = -5000)]
        [ScaffoldColumn(false)]
        public override string Validators
        {
            get
            {
                var captchaValidator = typeof(HcaptchaValidator).FullName;
                var validators = this.GetPropertyValue(content => content.Validators);
                if (string.IsNullOrWhiteSpace(validators))
                {
                    return captchaValidator;
                }
                else
                {
                    return string.Concat(validators, Forms.Constants.RecordSeparator, captchaValidator);
                }
            }
            set
            {
                this.SetPropertyValue(content => content.Validators, value);
            }
        }

        public override object GetSubmittedValue()
        {
            var httpContext = ServiceLocator.Current.GetInstance<IHttpContextAccessor>();
            if(httpContext.HttpContext.Request.Method == "POST")
            {
                return httpContext.HttpContext.Request.Form[this.Content.GetElementName()];
            }
            else
            {
                return httpContext.HttpContext.Request.Query[this.Content.GetElementName()];
            }
        }

        #endregion

        /// <summary>
        /// The score threshold of hCaptcha.
        /// </summary>
        [Display(GroupName = SystemTabNames.Content, Order = -3600)]
        [Range(Constants.MinimumRecaptchaScoreThreshold, Constants.MaximumRecaptchaScoreThreshold)]
        [Required]
        public virtual double ScoreThreshold
        {
            get; set;
        }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            ScoreThreshold = Constants.DefaultHcaptchaScoreThreshold;
        }

        /// <summary>
        /// The site key for HCAPTCHA element.
        /// </summary>
        [Display(GroupName = SystemTabNames.Content, Order = -3500)]
        public virtual string SiteKey
        {
            get
            {
                var siteKey = this.GetPropertyValue(content => content.SiteKey);
                if (string.IsNullOrWhiteSpace(siteKey))
                {
                    try
                    {
                        siteKey = _config.Service.HcaptchaKey?.SiteKey;
                    }
                    catch (ConfigurationErrorsException ex)
                    {
                        _logger.Warning("Cannot get RecaptchaSiteKey from app settings.", ex);
                    }
                }
                return siteKey;
            }
            set
            {
                this.SetPropertyValue(content => content.SiteKey, value);
            }
        }

        /// <summary>
        /// The shared key between site and HCAPTCHA.
        /// </summary>
        [Display(GroupName = SystemTabNames.Content, Order = -3400)]
        public virtual string SecretKey
        {
            get
            {
                var secretKey = this.GetPropertyValue(content => content.SecretKey);
                if (string.IsNullOrWhiteSpace(secretKey))
                {
                    try
                    {
                        secretKey = _config.Service.HcaptchaKey?.SecretKey;
                    }
                    catch (ConfigurationErrorsException ex)
                    {
                        _logger.Warning("Cannot get RecaptchaSecretKey from app settings.", ex);
                    }
                }
                return secretKey;
            }
            set
            {
                this.SetPropertyValue(content => content.SecretKey, value);
            }
        }

        public override string EditViewFriendlyTitle
        {
            get
            {
                var friendlyTitle = string.IsNullOrWhiteSpace(SiteKey) || string.IsNullOrWhiteSpace(SecretKey) ?
                                        string.Format("{0}: ({1})", base.EditViewFriendlyTitle, LocalizationService.GetString("/episerver/forms/editview/notconfigured"))
                                        : string.Format("{0}: ({1})", base.EditViewFriendlyTitle, LocalizationService.GetString("/episerver/forms/samples/editview/requirejsHcaptcha"));
                return friendlyTitle;
            }
        }

        public IEnumerable<Tuple<string, string>> GetExtraResources()
        {
            return new List<Tuple<string, string>>() {
                new Tuple<string, string>(
                    "script", string.Format("https://js.hcaptcha.com/1/api.js")
                )
            };
        }
    }
}