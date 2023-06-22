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
using EPiServer.Forms.Implementation.Elements.BaseClasses;
using EPiServer.Forms.Implementation.Validation;
using EPiServer.Forms.Samples.EditView;
using EPiServer.Forms.Samples.EditView.SelectionFactory;
using EPiServer.Forms.Samples.Implementation.Models;
using EPiServer.Forms.Samples.Implementation.Validation;
using EPiServer.Shell.ObjectEditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EPiServer.Forms.Samples.Implementation.Elements
{
    /// <summary>
    /// DateTime element for EpiForm (support Visitor picking Time, Date, or both Date and Time)
    /// </summary>
    [ContentType(GUID = "{3CC0755E-E50B-4AF4-92DF-C0F7625D526F}", GroupName = ConstantsFormsUI.FormElementGroup, Order = 2230)]
    [AvailableValidatorTypesAttribute(Include = new Type[] { typeof(RequiredValidator) })]
    public class DateTimeElementBlock : InputElementBlockBase, IElementCustomFormatValue, IElementRequireClientResources
    {
        [SelectOne(SelectionFactoryType = typeof(DateTimePickerTypeSelectionFactory))]
        [Display(GroupName = SystemTabNames.Content, Order = -6000)]
        public virtual int PickerType { get; set; }

        /// <summary>
        /// Always use a custom Validator to validate this datetime element (along with builtin Validator like RequiredValidator)
        /// <remarks>The datetime custom Validator is not visible to Editor (in EditView), but it still works to validate element value</remarks>
        /// </summary>
        [Display(GroupName = SystemTabNames.Content, Order = -5000)]
        public override string Validators
        {
            get
            {
                var pickerValidator = GetValidatorTypeForPicker((DateTimePickerType)PickerType).FullName;
                var validators = this.GetPropertyValue(content => content.Validators);
                if (string.IsNullOrEmpty(validators))
                {
                    return pickerValidator;
                }
                else
                {
                    return string.Concat(validators, EPiServer.Forms.Constants.RecordSeparator, pickerValidator);
                }
            }
            set
            {
                this.SetPropertyValue(content => content.Validators, value);
            }
        }


        /// <summary>
        /// Base on DateTimePicker type, we use appropriate Validator.
        /// </summary>
        private Type GetValidatorTypeForPicker(DateTimePickerType pickerType)
        {
            switch (pickerType)
            {
                case DateTimePickerType.DatePicker:
                    return typeof(DateValidator);
                case DateTimePickerType.TimePicker:
                    return typeof(TimeValidator);
                case DateTimePickerType.DateTimePicker:
                    return typeof(DateTimeValidator);

                default:
                    return null;
            }
        }

        /// <inheritdoc />
        public virtual object GetFormattedValue()
        {
            // NOTE: submittedValue is YYYY-MM-DDTHH:mmTZD(ISO-8601), might need to transform to date only (yyyy/MM/dd) or time only (hh:mm)

            var submittedValue = this.GetSubmittedValue();
            if (submittedValue == null)
            {
                return null;
            }

            var valueString = submittedValue.ToString();
            DateTimeOffset dateTimeOffsetValue;
            if (!DateTimeOffset.TryParse(valueString, out dateTimeOffsetValue))
            {
                return valueString;
            }
            var dateTimeSegments = dateTimeOffsetValue.DateTime.ToString("s", CultureInfo.InvariantCulture).Split(new char[] { 'T' }, StringSplitOptions.RemoveEmptyEntries);

            var pickerType = (DateTimePickerType)this.PickerType;
            switch (pickerType)
            {
                case DateTimePickerType.TimePicker:
                    return dateTimeSegments[1];
                case DateTimePickerType.DatePicker:
                    return dateTimeSegments[0];
                default:
                    return valueString;
            }
        }

        /// <inheritdoc />
        public override ElementInfo GetElementInfo()
        {
            var baseInfo = base.GetElementInfo();

            var dateTimeElementInfo = new DateTimeElementInfo
            {
                Type = baseInfo.Type,
                FriendlyName = baseInfo.FriendlyName,
                CustomBinding = true,
                PickerType = ((DateTimePickerType)this.PickerType).ToString().ToLower()
            };

            return dateTimeElementInfo;
        }

        public IEnumerable<Tuple<string, string>> GetExtraResources()
        {
            var publicVirtualPath = ModuleHelper.GetPublicVirtualPath(Constants.ModuleName);
            return new List<Tuple<string, string>>() {
               new Tuple<string, string>("script", publicVirtualPath + "/js/datetimepicker.modified.js"),
               new Tuple<string, string>("script", publicVirtualPath + "/js/DateTimeElementBlock.js")
            };
        }

        /// <summary>
        /// convert datetime string with format YYYY-MM-DDTHH:mmTZD(ISO-8601) -> [yyyy-MM-dd hh:mm tt] OR [hh:mm:ss] -> [hh:mm tt]
        /// </summary>
        /// <returns></returns>
        public override string GetDefaultValue()
        {
            var result = base.GetDefaultValue();// datetime string with format [YYYY-MM-DDTHH:mmTZD](ISO-8601) OR [YYYY-MM-DD] OR [hh:mm:ss]

            if (!string.IsNullOrEmpty(this.GetErrorMessage()))
            {
                return result;
            }

            var pickerType = (DateTimePickerType)this.PickerType;
            DateTime dateTimeValue;
            if (!DateTime.TryParse(result, out dateTimeValue))
            {
                return result;
            }

            switch (pickerType)
            {
                case DateTimePickerType.TimePicker:
                    return dateTimeValue.ToString("hh:mm tt");
                case DateTimePickerType.DateTimePicker:
                    return DateTimeOffset.Parse(result).ToString("yyyy-MM-dd hh:mm tt"); //ignore offset
                default:
                    return result;
            }
        }
    }
}
