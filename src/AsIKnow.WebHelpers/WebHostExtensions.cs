using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AsIKnow.WebHelpers
{
    public static class WebHostExtensions
    {
        private static string ConfigureMethodNamePrefix = "Configure";
        public static IWebHost AddEnvironmentOperations<TOperation>(this IWebHost ext) where TOperation : class
        {
            ext = ext ?? throw new ArgumentNullException(nameof(ext));

            ConstructorInfo[] ctor = typeof(TOperation).GetConstructors();
            if (ctor.Length != 1)
                throw new ArgumentException($"The type <{typeof(TOperation).Name}> should have a unique constructor.");
            if(!typeof(TOperation).GetMethods().Any(p=>p.Name.StartsWith("Configure")))
                throw new ArgumentException($"The type <{typeof(TOperation).Name}> should have at least a method with prefix name '{ConfigureMethodNamePrefix}'.");

            using (IServiceScope scope = ext.Services.CreateScope())
            {
                List<object> args = new List<object>();
                foreach (ParameterInfo item in ctor[0].GetParameters())
                {
                    args.Add(scope.ServiceProvider.GetRequiredService(item.ParameterType));
                }

                TOperation op = (TOperation)Activator.CreateInstance(typeof(TOperation), args.ToArray());

                IHostingEnvironment env = scope.ServiceProvider.GetRequiredService<IHostingEnvironment>();
                string configureMethodName = $"{ConfigureMethodNamePrefix}{env.EnvironmentName}";

                MethodInfo mInfo = typeof(TOperation).GetMethod(configureMethodName) ?? typeof(TOperation).GetMethod(ConfigureMethodNamePrefix);

                args.Clear();
                foreach (ParameterInfo item in mInfo.GetParameters())
                {
                    args.Add(scope.ServiceProvider.GetRequiredService(item.ParameterType));
                }
                mInfo.Invoke(op, args.ToArray());
            }

            return ext;
        }
    }
}
