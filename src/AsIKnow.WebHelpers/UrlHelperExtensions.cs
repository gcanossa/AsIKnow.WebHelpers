using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Linq;

namespace AsIKnow.WebHelpers
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Adjust the given uri with respect to the originating request.
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static Uri AdjustUri(this IUrlHelper urlHelper, Uri ext)
        {
            HttpRequest req = urlHelper.ActionContext.HttpContext.Request;
            string scheme = req.Headers.Any(p => p.Key.ToLower() == "x-forwarded-proto") ?
                req.Headers.First(p => p.Key.ToLower() == "x-forwarded-proto").Value[0]
                :
                req.Scheme;
            string host = req.Headers.Any(p => p.Key.ToLower() == "x-forwarded-host") ?
                req.Headers.First(p => p.Key.ToLower() == "x-forwarded-host").Value[0]
                :
                req.Host.Host;
            int port = req.Headers.Any(p => p.Key.ToLower() == "x-forwarded-port") ?
                Convert.ToInt32(req.Headers.First(p => p.Key.ToLower() == "x-forwarded-port").Value[0])
                :
                req.HttpContext.Connection.LocalPort;

            UriBuilder ub = new UriBuilder(ext)
            {
                Scheme = scheme,
                Host = host,
                Port = port
            };

            return ub.Uri;
        }

        /// <summary>
        /// Adjust the given object uri with respect to the originating request.
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string AdjustStringUri(this IUrlHelper urlHelper, string ext)
        {
            return urlHelper.AdjustUri(new Uri(ext)).ToString();
        }

        /// <summary>
        /// Adjust the give uri properties with respect to the originating request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlHelper"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static T AdjustObjectUriProperties<T>(this IUrlHelper urlHelper, T ext) where T : new()
        {
            T result = (T)Activator.CreateInstance(ext.GetType());
            foreach (PropertyInfo pinfo in ext.GetType().GetProperties().Where(p => p.CanWrite))
            {
                if (pinfo.PropertyType == typeof(Uri))
                    pinfo.SetValue(result, urlHelper.AdjustUri((Uri)pinfo.GetValue(ext)));
                else
                    pinfo.SetValue(result, pinfo.GetValue(ext));
            }

            return result;
        }
    }
}
