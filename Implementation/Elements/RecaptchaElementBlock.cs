using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Internal;
using EPiServer.Forms.EditView;
using EPiServer.Forms.Helpers.Internal;
using EPiServer.Forms.Implementation.Elements.BaseClasses;
using EPiServer.Forms.Samples.Implementation.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
                if (string.IsNullOrEmpty(validators))
                {
                    return captchaValidator;
                }
                else
                {
                    return string.Concat(validators, EPiServer.Forms.Constants.RecordSeparator, captchaValidator);
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
            var submitData = HttpContext.Current.Request.HttpMethod == "POST" ? HttpContext.Current.Request.Form : HttpContext.Current.Request.QueryString;
            return submitData["g-recaptcha-response"];
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
        public virtual string SiteKey { get; set; }

        /// <summary>
        /// The shared key between site and ReCAPTCHA.
        /// </summary>
        [Display(GroupName = SystemTabNames.Content, Order = -3400)]
        public virtual string SecretKey { get; set; }

        /// <inheritdoc />
        public override string EditViewFriendlyTitle
        {
            get
            {
                var siteKey = this.GetPropertyValue(content => content.SiteKey);
                var secret = this.GetPropertyValue(content => content.SecretKey);

                var friendlyTitle = string.IsNullOrEmpty(siteKey) || string.IsNullOrEmpty(secret) ?
                                        string.Format("{0}: ({1})", base.EditViewFriendlyTitle, LocalizationService.GetString("/episerver/forms/editview/notconfigured"))
                                        : string.Format("{0}: ({1})", base.EditViewFriendlyTitle, LocalizationService.GetString("/episerver/forms/samples/editview/requirejs"));
                return friendlyTitle;
            }
        }

        public IEnumerable<Tuple<string, string>> GetExtraResources()
        {
            var currentPageLanguage = FormsExtensions.GetCurrentPageLanguage();
            var publicVirtualPath = ModuleHelper.GetPublicVirtualPath(Constants.ModuleName);
            return new List<Tuple<string, string>>() {
                new Tuple<string, string>("script", string.Format("https://www.google.com/recaptcha/api.js?onload=initRecaptchaElements&render=explicit&hl={0}", currentPageLanguage)),
                new Tuple<string, string>("script", publicVirtualPath + "/ClientResources/ViewMode/RecaptchaElementBlock.js")
            };
        }
    }
}
