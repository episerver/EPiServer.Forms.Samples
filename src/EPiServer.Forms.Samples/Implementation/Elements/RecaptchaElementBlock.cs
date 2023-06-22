using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Internal;
using EPiServer.Forms.EditView;
using EPiServer.Forms.Helpers.Internal;
using EPiServer.Forms.Implementation.Elements.BaseClasses;
using EPiServer.Forms.Samples.Configuration;
using EPiServer.Forms.Samples.Implementation.Models;
using EPiServer.Forms.Samples.Implementation.Validation;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Web;

namespace EPiServer.Forms.Samples.Implementation.Elements
{
    /// <summary>
    /// Captcha element using Goolge reCAPTCHA. This element requires js, in non-js mode the validation will be failed.
    /// For get site key and secret key go to: https://www.google.com/recaptcha/admin#list and register your site.
    /// </summary>
    [ContentType(GUID = "{2D7E4A18-8F8B-4C98-9E81-D97524C62561}", GroupName = ConstantsFormsUI.FormElementGroup, Order = 2910)]
    public class RecaptchaElementBlock : ValidatableElementBlockBase, IExcludeInSubmission, IViewModeInvisibleElement, IElementRequireClientResources
    {
        private static readonly ILogger _logger = LogManager.GetLogger(typeof(RecaptchaElementBlock));
        private Injected<FormSamplesApiKeyOptions> _config;

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
                var captchaValidator = typeof(RecaptchaValidator).FullName;
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

        /// <inheritdoc />
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

        /// <summary>
        /// The site key for ReCAPTCHA element.
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
                        siteKey = _config.Service.RecaptchaKey?.SiteKey;
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
        /// The shared key between site and ReCAPTCHA.
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
                        secretKey = _config.Service.RecaptchaKey?.SecretKey;
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

        /// <summary>
        /// The score threshold of reCaptcha.
        /// </summary>
        [Display(GroupName = SystemTabNames.Content, Order = -3600) ]
        [Range(Constants.MinimumRecaptchaScoreThreshold, Constants.MaximumRecaptchaScoreThreshold)]
        [Required]
        public virtual double ScoreThreshold
        {
            get; set;
        }

        /// <inheritdoc />
        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            ScoreThreshold = Constants.DefaultRecaptchaScoreThreshold;
        }

        /// <inheritdoc />
        public override string EditViewFriendlyTitle
        {
            get
            {
                var friendlyTitle = string.IsNullOrWhiteSpace(SiteKey) || string.IsNullOrWhiteSpace(SecretKey) ?
                                        string.Format("{0}: ({1})", base.EditViewFriendlyTitle, LocalizationService.GetString("/episerver/forms/editview/notconfigured"))
                                        : string.Format("{0}: ({1})", base.EditViewFriendlyTitle, LocalizationService.GetString("/episerver/forms/samples/editview/requirejsReCAPTCHA"));
                return friendlyTitle;
            }
        }

        public IEnumerable<Tuple<string, string>> GetExtraResources()
        {
            return new List<Tuple<string, string>>() {
                new Tuple<string, string>("script", string.Format("https://www.google.com/recaptcha/api.js?render={0}", SiteKey))
            };
        }
    }
}
