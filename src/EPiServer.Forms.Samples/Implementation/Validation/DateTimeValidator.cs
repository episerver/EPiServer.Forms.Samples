using EPiServer.Forms.Core.Models;
using EPiServer.Forms.Core.Validation;
using EPiServer.Forms.Helpers.Internal;
using EPiServer.Forms.Samples.EditView;
using EPiServer.Forms.Samples.Implementation.Elements;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using System;
using System.Text.RegularExpressions;

namespace EPiServer.Forms.Samples.Implementation.Validation
{
    /// <summary>
    ///     validate date time string in format YYYY-MM-DDTHH:mmTZD(ISO-8601)
    /// </summary>
    public class DateTimeValidatorBase : ElementValidatorBase
    {
        private Injected<LocalizationService> _localizationService;
        protected LocalizationService LocalizationService { get { return _localizationService.Service; } }

        /// <inheritdoc />
        public override bool? Validate(IElementValidatable targetElement)
        {
            var submittedValue = targetElement.GetSubmittedValue() as string;
            if (string.IsNullOrEmpty(submittedValue))
            {
                return true;
            }

            DateTime dateTime;
            if (DateTime.TryParse(submittedValue, out dateTime))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc />
        /// we don't want to show this specific-validators to Editor. This validators always work programatically for DateTimeElement.
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

    public class DateTimeRangeValidator : ElementValidatorBase
    {
        private Injected<LocalizationService> _localizationService;
        protected LocalizationService LocalizationService { get { return _localizationService.Service; } }

        /// <inheritdoc />
        public override bool? Validate(IElementValidatable targetElement)
        {
            // DateTime format: YYYY-MM-DDTHH:mmTZD(ISO-8601) "2018-01-01T14:06+07:00|2018-02-02T14:06+07:00"

            var submittedValues = (targetElement.GetSubmittedValue() as string[]);
            // if the value is null, then let the RequiredValidator do the work
            if (submittedValues == null)
            {
                return true;
            }

            if (submittedValues.Length < 2)
            {
                return false;
            }
            DateTime startDateTime;
            DateTime endDateTime;
            var isStartDateValid = DateTime.TryParse(submittedValues[0], out startDateTime);
            var isEndDateValid = DateTime.TryParse(submittedValues[1], out endDateTime);
            if (!isStartDateValid || !isEndDateValid)
            {
                return false;
            }
            var datetimeRangeBlock = targetElement as DateTimeRangeElementBlock;
            var pickerType = (datetimeRangeBlock != null) ? (DateTimePickerType)datetimeRangeBlock.PickerType : DateTimePickerType.DateTimePicker;
            switch (pickerType)
            {
                case DateTimePickerType.DatePicker:
                    return endDateTime.Subtract(startDateTime).TotalDays > 0;
            }
            return endDateTime.Subtract(startDateTime).TotalSeconds > 0;
        }

        /// <inheritdoc />
        /// we don't want to show this specific-validators to Editor. This validators always work programatically for DateTimeRangeElement.
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
                model.Message = this.LocalizationService.GetString("/episerver/forms/validators/episerver.forms.implementation.validation.datetimerangevalidator/message");
            }

            return model;
        }
    }

    public class DateTimeValidator : DateTimeValidatorBase {}

    public class DateValidator : DateTimeValidatorBase {}

    public class TimeValidator : DateTimeValidatorBase
    {
        public override bool? Validate(IElementValidatable targetElement)
        {
            // DateTime format: YYYY-MM-DDTHH:mmTZD(ISO-8601) "2018-01-01T14:06+07:00"
            var submittedValue = targetElement.GetSubmittedValue() as string;
            if (string.IsNullOrEmpty(submittedValue))
            {
                return true;
            }

            DateTime dateTime;
            if (!DateTime.TryParse(submittedValue, out dateTime))
            {
                return false;
            }

            return true;
        }
    }
}