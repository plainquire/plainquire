using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;

namespace FS.FilterExpressionCreator.Demo.Extensions
{
    internal static class HostEnvironmentExtensions
    {
        private static readonly Assembly _entryAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

        public static string GetWebRootPath(this IHostEnvironment hostEnvironment)
        {
            var distWebRootPath = Path.Combine(hostEnvironment.ContentRootPath, _entryAssembly.GetName().Name, "dist");
            if (Directory.Exists(distWebRootPath))
                return distWebRootPath;

            var debugWebRootPath = Path.Combine(hostEnvironment.ContentRootPath, "wwwroot");
            if (Directory.Exists(debugWebRootPath))
                return debugWebRootPath;

            return null;
        }
    }
}
