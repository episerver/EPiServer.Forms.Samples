using EPiServer.Framework.Web.Resources;
using EPiServer.Shell.Modules;

namespace EPiServer.Forms.Samples
{
    /// <summary>
    /// Shell module for (gadgets of EPiServer Forms) working in EditView
    /// </summary>
    public class FormsSamplesModule : ShellModule
    {
        public FormsSamplesModule(string name, string routeBasePath, string resourceBasePath)
            : base(name, routeBasePath, resourceBasePath)
        { }

        public override ModuleViewModel CreateViewModel(ModuleTable moduleTable, IClientResourceService clientResourceService)
        {
            return new FormsSamplesModuleViewModel(this, clientResourceService);
        }
    }
}