using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace WebMVC.Infrastructure
{
    public class WebContextSeed
    {
        public static void Seed(IApplicationBuilder applicationBuilder, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var log = loggerFactory.CreateLogger<WebContextSeed>();

            var settings = (AppSettings)applicationBuilder
                .ApplicationServices.GetRequiredService<IOptions<AppSettings>>().Value;

            var useCustomizationData = settings.UseCustomizationData;
            var contentRootPath = env.ContentRootPath;
            var webroot = env.WebRootPath;

            if (useCustomizationData)
            {
                GetPreconfiguredCSS(contentRootPath, webroot, log);
            }
        }

        static void GetPreconfiguredCSS(string contentRootPath, string webroot, ILogger log)
        {
            try
            {
                string overrideCssFile = Path.Combine(contentRootPath, "Setup", "override.css");
                if (!File.Exists(overrideCssFile))
                {
                    log.LogError("Override css file '{FileName}' does not exists.", overrideCssFile);
                    return;
                }

                string destinationFilename = Path.Combine(webroot, "css", "override.css");
                File.Copy(overrideCssFile, destinationFilename, true );
            }
            catch (Exception ex)
            {
                log.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            }
        }

    }

}
