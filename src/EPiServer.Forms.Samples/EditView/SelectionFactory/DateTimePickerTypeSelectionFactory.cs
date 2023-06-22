using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors.SelectionFactories;
using EPiServer.Framework.Localization;
using EPiServer.Shell.ObjectEditing;
using System;

namespace EPiServer.Forms.Samples.EditView.SelectionFactory
{
    /// <summary>
    /// Creates selections for enum of type <see cref="DateTimePickerType"/>.
    /// </summary>
    [SelectionFactoryRegistration]
    public class DateTimePickerTypeSelectionFactory : EnumSelectionFactory
    {
        public DateTimePickerTypeSelectionFactory() : this(LocalizationService.Current) { }

        public DateTimePickerTypeSelectionFactory(LocalizationService localizationService) : base(localizationService) { }

        protected override Type EnumType
        {
            get
            {
                return typeof(DateTimePickerType);
            }
        }

        protected override string GetStringForEnumValue(int value)
        {
            return LocalizationService.GetString("/episerver/forms/samples/datetimepickertype/" + Enum.GetName(EnumType, value).ToLowerInvariant());
        }
    }
}