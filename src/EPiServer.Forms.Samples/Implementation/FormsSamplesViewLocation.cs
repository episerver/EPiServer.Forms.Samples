
using EPiServer.Forms.EditView;
using EPiServer.ServiceLocation;

namespace EPiServer.Forms.Samples.Implementation
{
    /// <summary>
    /// This tell EpiForm that template of Samples (cshtml files) should be scanned from Samples addon folder.
    /// </summary>
    [ServiceConfiguration(typeof(ICustomViewLocation))]
    public class FormsSamplesViewLocation : CustomViewLocationBase
    {
        /// <summary>
        /// This will be loaded before path of Form.Core
        /// </summary>
        public override int Order { get { return 500; } set { } }
        public override string[] Paths
        {
            get
            {
                return new string[] { GetDefaultViewFormsSampleLocation() };
            }
        }

        public string GetDefaultViewFormsSampleLocation()
        {
            return "Views/ElementBlocks";
        }
    }
}
