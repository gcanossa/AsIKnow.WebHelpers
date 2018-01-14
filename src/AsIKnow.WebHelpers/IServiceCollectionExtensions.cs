using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace AsIKnow.WebHelpers
{
    public static class IServiceCollectionExtensions
    {
        public static void AddWebAppOptions(this IServiceCollection ext, WebApplicationOptions options = null)
        {
            ext = ext ?? throw new ArgumentNullException(nameof(ext));
            options = options ?? new WebApplicationOptions();

            IDataProtectionBuilder dpBuilder = ext.AddDataProtection()
                .SetApplicationName(options.DataProtection.ApplicationName)
                .PersistKeysToFileSystem(new DirectoryInfo(options.DataProtection.KeyRingPath));
            if (options.DataProtection.DisableAutomaticKeyGeneration)
                dpBuilder.DisableAutomaticKeyGeneration();
        }
    }
}
