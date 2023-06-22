using EPiServer.Forms.Helpers.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace EPiServer.Forms.Samples
{
    public class FormsSamplePublicStaticFileStartupFilter : IStartupFilter
    {
        public System.Action<IApplicationBuilder> Configure(System.Action<IApplicationBuilder> next) =>
            app =>
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    RequestPath = ModuleHelper.GetPublicVirtualPath("EPiServer.Forms.Samples"),
                    FileProvider = new ManifestEmbeddedFileProvider(typeof(FormsSamplePublicStaticFileStartupFilter).Assembly, "/ClientResources/ViewMode")
                }); 

                next(app);
            };
    }
}
