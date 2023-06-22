using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Internal;
using EPiServer.Forms.Core.Models.Internal;
using EPiServer.Forms.EditView;
using EPiServer.Forms.EditView.Models.Internal;
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
using EPiServer.Forms.Helpers.Internal;
using EPiServer.Forms.EditView.DataAnnotations;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace EPiServer.Forms.Samples.Implementation.Elements
{
    /// <summary>
    /// DateTimeRange element for EpiForm (support Visitor picking Time range, Date range)
    /// </summary>
    [ContentType(GUID = "{A4EE6053-3932-4300-8B3B-7BABF9AEC512}", GroupName = ConstantsFormsUI.FormElementGroup, Order = 2230)]
    [AvailableValidatorTypesAttribute(Include = new Type[] { typeof(RequiredValidator) })]
    public class DateTimeRangeElementBlock : InputElementBlockBase, IElementCustomFormatValue, IElementRequireClientResources
    {
        [SelectOne(SelectionFactoryType = typeof(DateTimePickerTypeSelectionFactory))]
        [Display(GroupName = SystemTabNames.Content, Order = -6000)]
        public virtual int PickerType { get; set; }

        /// <summary>
        /// Always use a custom Validator to validate this datetime range element (along with builtin Validator like RequiredValidator)
        /// <remarks>The datetime range custom Validator is not visible to Editor (in EditView), but it still works to validate element value</remarks>
        /// </summary>
        [Display(GroupName = SystemTabNames.Content, Order = -5000)]
        public override string Validators
        {
            get
            {
                var pickerValidator = typeof(DateTimeRangeValidator).FullName;
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

        /// <inheritdoc />
        public override object GetSubmittedValue()
        {
            string[] separatingStrings = { "|" };
            var submittedValue = base.GetSubmittedValue().ToString() ?? string.Empty;

            var datetimes = submittedValue.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            if (datetimes.All(datetime => string.IsNullOrWhiteSpace(datetime)))
            {
                return null;
            }
            return datetimes;
        }

        /// <inheritdoc />
        public virtual object GetFormattedValue()
        {
            // NOTE: submittedValue is an string with format YYYY-MM-DDTHH:mmTZD(ISO-8601) | YYYY-MM-DDTHH:mmTZD(ISO-8601) or YYYY-MM-DDTHH:mmTZD(ISO-8601) , YYYY-MM-DDTHH:mmTZD(ISO-8601)
            // might need to transform to date only (yyyy/MM/dd | yyyy/MM/dd) or time only (hh:mm | hh:mm)
            var submittedValues = GetSubmittedValue() as string[];

            if (submittedValues == null || submittedValues.All(string.IsNullOrWhiteSpace))
            {
                return null;
            }
            var formattedValues = submittedValues.Select(value =>
            {
                var valueString = value.ToString();
                DateTimeOffset dateTimeValue;
                if (!DateTimeOffset.TryParse(valueString, out dateTimeValue))
                {
                    return valueString;
                }
                var dateTimeConvert = dateTimeValue.DateTime.ToString("s", CultureInfo.InvariantCulture);
                var dateTimeSegments = dateTimeConvert.Split(new char[] { 'T' }, StringSplitOptions.RemoveEmptyEntries);
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
            });

            return string.Join("|", formattedValues);
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

        public string GetDateTimeDefaultValue()
        {
            return GetDefaultValue()?.Split('|', ',').Select(ConvertToClientDateTime).ToStringWithSeparator("|");
        }
        // datetime in server should have this format YYYY-MM-DDTHH:mmTZD(ISO-8601) | YYYY-MM-DDTHH:mmTZD(ISO-8601)
        // might need to transform to date only(yyyy/MM/dd | yyyy/MM/dd) or time only(hh:mm | hh:mm)
        protected virtual string ConvertToClientDateTime(string datetimeString)
        {
            DateTime dateTimeValue;
            if (!DateTime.TryParse(datetimeString, out dateTimeValue))
            {
                return string.Empty;
            }

            var pickerType = (DateTimePickerType)this.PickerType;
            switch (pickerType)
            {
                case DateTimePickerType.TimePicker:
                    return dateTimeValue.ToString("hh:mm tt");
                case DateTimePickerType.DateTimePicker:
                    return DateTimeOffset.Parse(datetimeString).ToString("yyyy-MM-dd hh:mm tt"); //ignore offset
                default:
                    return datetimeString;
            }
        }

        public IEnumerable<Tuple<string, string>> GetExtraResources()
        {
            var publicVirtualPath = ModuleHelper.GetPublicVirtualPath(Constants.ModuleName);
            return new List<Tuple<string, string>>() {
               new Tuple<string, string>("script", publicVirtualPath + "/js/datetimepicker.modified.js"),
               new Tuple<string, string>("script", publicVirtualPath + "/js/DateTimeElementBlock.js")
            };
        }
    }
}
