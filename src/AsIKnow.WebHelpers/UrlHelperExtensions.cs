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
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Uri AdjustUri(this IUrlHelper urlHelper, Uri uri)
        {
            uri = uri ?? throw new ArgumentException(nameof(uri));

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

            UriBuilder ub = new UriBuilder(uri)
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
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string AdjustStringUri(this IUrlHelper urlHelper, string uri)
        {
            return urlHelper.AdjustUri(new Uri(uri)).ToString();
        }

        /// <summary>
        /// Adjust the given uri properties with respect to the originating request, modifying the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlHelper"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T AdjustObjectUriProperties<T>(this IUrlHelper urlHelper, T obj) where T : class
        {
            if(obj == null)
                throw new ArgumentNullException(nameof(obj));
            
            foreach (PropertyInfo pinfo in obj.GetType().GetProperties().Where(p => p.CanWrite))
            {
                if (pinfo.PropertyType == typeof(Uri))
                    pinfo.SetValue(obj, urlHelper.AdjustUri((Uri)pinfo.GetValue(obj)));
            }

            return obj;
        }
    }
}
