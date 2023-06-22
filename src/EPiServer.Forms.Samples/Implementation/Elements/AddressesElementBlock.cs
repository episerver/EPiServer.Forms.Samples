using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Internal;
using EPiServer.Forms.Core.Models.Internal;
using EPiServer.Forms.EditView;
using EPiServer.Forms.EditView.DataAnnotations;
using EPiServer.Forms.EditView.Models.Internal;
using EPiServer.Forms.Helpers.Internal;
using EPiServer.Forms.Implementation;
using EPiServer.Forms.Implementation.Elements.BaseClasses;
using EPiServer.Forms.Implementation.Validation;
using EPiServer.Forms.Samples.EditView;
using EPiServer.Forms.Samples.EditView.SelectionFactory;
using EPiServer.Forms.Samples.Implementation.Models;
using EPiServer.Forms.Samples.Implementation.Validation;
using EPiServer.Shell.ObjectEditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using EPiServer.Configuration;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;
using EPiServer.Forms.Samples.Configuration;
using System.Configuration;
using EPiServer.Logging;
using EPiServer.Forms.Samples.Controllers;

namespace EPiServer.Forms.Samples.Implementation.Elements
{
    /// <summary>
    /// Addresses element for EpiForm (support Visitor picking address)
    /// </summary>
    [ContentType(GUID = "{CB332A5D-096D-4228-BE9A-B6E16481D8FB}", GroupName = ConstantsFormsUI.FormElementGroup, Order = 2230)]
    public class AddressesElementBlock : InputElementBlockBase, IElementCustomFormatValue, IElementRequireClientResources
    {
        private Injected<FormSamplesApiKeyOptions> _config;
        private static readonly ILogger _logger = LogManager.GetLogger(typeof(AddressesElementBlock));

        /// <inheritdoc />
        [Ignore]
        public override string PlaceHolder { get; set; }

        /// <inheritdoc />
        [Ignore]
        public override string PredefinedValue { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = -6100)]
        [Range(100, 350)]
        public virtual int MapWidth { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = -6200)]
        [Range(100, 350)]
        public virtual int MapHeight { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = -6210)]
        public virtual string ApiKey
        {
            get
            {
                var apiKey = this.GetPropertyValue(content => content.ApiKey);
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    try
                    {
                        apiKey = _config.Service.AddressKey?.ClientApiKey;
                    }
                    catch (ConfigurationErrorsException ex)
                    {
                        _logger.Warning("Cannot get ClientApiKey from app settings.", ex);
                    }
                }
                return apiKey;
            }
            set
            {
                this.SetPropertyValue(content => content.ApiKey, value);
            }
        }

        [Display(GroupName = SystemTabNames.Content, Order = -6300)]
        public virtual string AddressLabel { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = -6400)]
        public virtual string StreetLabel { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = -6500)]
        public virtual string CityLabel { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = -6600)]
        public virtual string StateLabel { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = -6700)]
        public virtual string PostalLabel { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = -6800)]
        public virtual string CountryLabel { get; set; }

        /// <summary>
        /// Always use AddressValidator to validate this element
        /// <remarks>hide from EditView</remarks>
        /// </summary>
        [Display(GroupName = SystemTabNames.Content, Order = -5000)]
        [ScaffoldColumn(false)]
        public override string Validators
        {
            get
            {
                var validator = typeof(AddressValidator).FullName;
                var validators = this.GetPropertyValue(content => content.Validators);
                if (string.IsNullOrEmpty(validators))
                {
                    return validator;
                }
                else
                {
                    return string.Concat(validators, Forms.Constants.RecordSeparator, validator);
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
            var rawSubmittedData = httpContext.HttpContext.Request.Form;
            var isJavaScriptSupport = rawSubmittedData[EPiServer.Forms.Constants.FormWithJavaScriptSupport];
            if (isJavaScriptSupport == "true")
            {
                return base.GetSubmittedValue();
            }
            string[] addressComponents = rawSubmittedData[this.Content.GetElementName()];
            if (addressComponents == null || addressComponents.Length < 1)
            {
                return null;
            }
            // NOTE: submittedValue is an string with format: address_detail | street | city | state | postal_code | country
            AddressInfo addressObj = new AddressInfo()
            {
                address = addressComponents.Length > 0 ? addressComponents[0] : null,
                street = addressComponents.Length > 1 ? addressComponents[1] : null,
                city = addressComponents.Length > 2 ? addressComponents[2] : null,
                state = addressComponents.Length > 3 ? addressComponents[3] : null,
                postalCode = addressComponents.Length > 4 ? addressComponents[4] : null,
                country = addressComponents.Length > 5 ? addressComponents[5] : null
            };

            return addressObj.ToJson();
        }

        /// <inheritdoc />
        public virtual object GetFormattedValue()
        {
            var submittedValue = GetSubmittedValue().ToString() ?? string.Empty;
            return submittedValue;
        }

        public virtual AddressInfo GetDefaultAddressInfo()
        {
            var defaultValue = GetDefaultValue();
            return defaultValue?.ToObject<AddressInfo>() ?? new AddressInfo();
        }

        /// <summary>
        /// Set default values for this block
        /// </summary>
        /// <param name="contentType"></param>
        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            MapWidth = Constants.DefaultMapImageWidth;
            MapHeight = Constants.DefaultMapImageHeight;
            AddressLabel = LocalizationService.GetString("/episerver/forms/viewmode/addresselement/addresslabel");
            StreetLabel = LocalizationService.GetString("/episerver/forms/viewmode/addresselement/streetlabel");
            CityLabel = LocalizationService.GetString("/episerver/forms/viewmode/addresselement/citylabel");
            StateLabel = LocalizationService.GetString("/episerver/forms/viewmode/addresselement/statelabel");
            PostalLabel = LocalizationService.GetString("/episerver/forms/viewmode/addresselement/postallabel");
            CountryLabel = LocalizationService.GetString("/episerver/forms/viewmode/addresselement/countrylabel");
        }

        /// <inheritdoc />
        public override ElementInfo GetElementInfo()
        {
            var baseInfo = base.GetElementInfo();
            baseInfo.CustomBinding = true;
            return baseInfo;
        }

        public IEnumerable<Tuple<string, string>> GetExtraResources()
        {
            var url = FormSamplesGoogleUrl.GoogleMapsApiV3Url;
            if (!string.IsNullOrWhiteSpace(url))
            {
                var publicVirtualPath = ModuleHelper.GetPublicVirtualPath(Constants.ModuleName);
                var currentPageLanguage = FormsExtensions.GetCurrentPageLanguage();
                var map_url = url + "&key=" + ApiKey;
                return new List<Tuple<string, string>>() {
                    new Tuple<string, string>("script", string.Format(map_url, currentPageLanguage)),
                    new Tuple<string, string>("script", publicVirtualPath + "/js/AddressesElementBlock.js")
                };
            }
            return new List<Tuple<string, string>>();
        }
    }
}
