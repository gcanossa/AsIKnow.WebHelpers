using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Http;

namespace AsIKnow.WebHelpers
{
    public static class DataExtensions
    {
        public static byte[] Inflate(this byte[] ext)
        {
            if (ext == null)
                throw new ArgumentNullException(nameof(ext));

            using (MemoryStream ms = new MemoryStream(ext))
            using (MemoryStream result = new MemoryStream())
            using (DeflateStream inf = new DeflateStream(ms, CompressionMode.Decompress))
            {
                inf.CopyTo(result);
                result.Flush();

                return result.ToArray();
            }
        }
        public static byte[] Deflate(this byte[] ext, CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            if (ext == null)
                throw new ArgumentNullException(nameof(ext));

            using (MemoryStream ms = new MemoryStream(ext))
            using (MemoryStream result = new MemoryStream())
            using (DeflateStream def = new DeflateStream(result, compressionLevel))
            {
                ms.CopyTo(def);
                def.Flush();

                return result.ToArray();
            }
        }

        public static bool IsBase64(this string ext)
        {
            if (ext == null)
                throw new ArgumentNullException(nameof(ext));

            return ext.Length % 4 == 0 && Regex.IsMatch(ext.Replace('-', '+').Replace('_', '/'), "^[a-zA-Z0-9\\+/]+=*$");
        }
        public static string ToBase64(this byte[] ext)
        {
            if (ext == null)
                throw new ArgumentNullException(nameof(ext));

            return Convert.ToBase64String(ext);
        }
        public static byte[] FromBase64(this string ext)
        {
            if (ext == null)
                throw new ArgumentNullException(nameof(ext));
            if (!ext.IsBase64())
                throw new ArgumentException("Not a valid base64 string.", nameof(ext));

            return Convert.FromBase64String(ext);
        }
        
        public static PaginatedList<T> PaginatedResponse<T>(this HttpRequest request, IEnumerable<T> items, int defaultPerPage = 15, int defaultPage = 1, Func<T, object> transform = null)
        {
            return PaginatedList<T>.FormRequest(request, items, defaultPerPage, defaultPage, transform);
        }
    }
}
