using EPiServer.Configuration;
using EPiServer.Forms.Core.Validation;
using EPiServer.Forms.Helpers.Internal;
using EPiServer.Forms.Implementation.Validation;
using EPiServer.Forms.Samples.Business;
using EPiServer.Forms.Samples.Implementation.Elements;
using System.Net;
using System;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System.Web;
using EPiServer.Forms.Core.Models;
using EPiServer.Framework.Localization;
using EPiServer.Forms.Samples.Configuration;
using System.Net.Http;
using System.Text.Json;

namespace EPiServer.Forms.Samples.Implementation.Validation
{
    public class AddressValidator: InternalElementValidatorBase
    {        
        private Injected<LocalizationService> _localizationService;
        protected LocalizationService LocalizationService { get { return _localizationService.Service; } }

        public override bool? Validate(IElementValidatable targetElement)
        {
            // if in js mode, the validation is done by an extenal service
            // if in non-js mode, just accept user input
            // so here, always return true
            return true;
        }

        /// <inheritdoc />
        /// we don't want to show this specific-validators to Editor. This validators always work programatically for AddressElement.
        public override bool AvailableInEditView
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc />
        public override IValidationModel BuildValidationModel(IElementValidatable targetElement)
        {
            var model = base.BuildValidationModel(targetElement);
            if (model != null)
            {
                model.Message = this.LocalizationService.GetString("/episerver/forms/validators/elementselfvalidator/unexpectedvalueisnotaccepted");
            }

            return model;
        }
    }
}
