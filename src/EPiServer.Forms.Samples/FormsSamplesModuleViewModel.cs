using EPiServer.Forms.Samples.Implementation.Elements;
using EPiServer.Framework.Web.Resources;
using EPiServer.Shell.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Forms.Samples
{
    /// <summary>
    /// ViewModel for working in EditView
    /// </summary>
    public class FormsSamplesModuleViewModel: ModuleViewModel
    {
        string _clientResourcePath;

        public FormsSamplesModuleViewModel(ShellModule shellModule, IClientResourceService clientResourceService)
            : base(shellModule, clientResourceService)
        {
            _clientResourcePath = shellModule.ClientResourcePath;
        }

        /// <summary>
        /// Returns root (to the version folder level) client resources path of this addon module.
        /// </summary>
        public string ClientResourcePath
        {
            get
            {
                return _clientResourcePath;
            }
        }

        /// <summary>
        /// Registered element content types existing in Samples.
        /// </summary>
        public IEnumerable<Type> RegisteredElementContentTypes
        {
            get
            {
                return new Type[] 
                {
                    typeof(HcaptchaElementBlock),     
                    typeof(AddressesElementBlock),
                    typeof(DateTimeElementBlock),
                    typeof(DateTimeRangeElementBlock),
                    typeof(RecaptchaElementBlock)
                };
            }
        }
    }
}
