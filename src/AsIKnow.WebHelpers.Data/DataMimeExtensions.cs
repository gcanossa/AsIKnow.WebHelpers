using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AsIKnow.WebHelpers.Data
{
    public static class DataMimeExtensions
    {
        private class MimeMarkers
        {
            private Func<MimeMarkers, byte[], bool> _additionalCheck;
            public MimeMarkers(string mimeType, string magicNumber, string trailer, Func<MimeMarkers, byte[], bool> additionalCheck = null)
            {
                Mime = mimeType;
                MagicNumber = magicNumber.Replace(" ", "").FromHexString();
                Trailer = trailer?.Replace(" ", "").FromHexString() ?? new byte[0];
                _additionalCheck = additionalCheck;
            }
            public string Mime { get; set; }
            public byte[] MagicNumber { get; private set; }
            public byte[] Trailer { get; private set; }

            public bool CheckMime(byte[] data)
            {
                return data != null &&
                    data.Length >= MagicNumber.Length + Trailer.Length &&
                    data.Take(MagicNumber.Length).SequenceEqual(MagicNumber) &&
                    (Trailer.Length == 0 || data.Skip(data.Length - Trailer.Length).Take(Trailer.Length).SequenceEqual(Trailer)) &&
                    (_additionalCheck == null || _additionalCheck(this, data));
            }

        }

        private class MimeNameComparer : IEqualityComparer<MimeMarkers>
        {
            public bool Equals(MimeMarkers x, MimeMarkers y)
            {
                return x.Mime == y.Mime;
            }

            public int GetHashCode(MimeMarkers obj)
            {
                return obj.Mime.GetHashCode();
            }
        }

        //https://www.garykessler.net/library/file_sigs.html
        private static List<MimeMarkers> _markers = new List<MimeMarkers>()
        {
            new MimeMarkers("image/png", "89 50 4E 47 0D 0A 1A 0A", "49 45 4E 44 AE 42 60 82"),
            new MimeMarkers("image/jpeg", "FF D8", "FF D9"),
            new MimeMarkers("image/bmp", "42 4D", null),
            new MimeMarkers("image/gif", "47 49 46 38 37 61", "00 3B"),
            new MimeMarkers("image/gif", "47 49 46 38 39 61", "00 3B"),
            new MimeMarkers("image/tiff", "49 20 49", null),
            new MimeMarkers("image/tiff", "49 49 2A 00", null),
            new MimeMarkers("image/tiff", "4D 4D 00 2A", null),
            new MimeMarkers("image/tiff", "4D 4D 00 2B", null),
            new MimeMarkers("image/svg+xml", "3C", "3E", (m, d) => Regex.IsMatch(Encoding.UTF8.GetString(d), "<svg.+</svg>")),

            new MimeMarkers("video/mp4", "66 74 79 70 4D 53 4E 56", null),
            new MimeMarkers("video/quicktime", "66 74 79 70 71 74 20 20", null),
            new MimeMarkers("video/quicktime", "6D 6F 6F 76", null),

            new MimeMarkers("application/pdf", "25 50 44 46", null, (m, d)=>
                    d.Length >= 4 &&
                    d.Take(1024).Select((v,i)=>new { i=i, v=v })
                    .Where(p=>p.v==m.MagicNumber[0])
                    .Select(p=>p.i)
                    .Any(p=>d.Skip(p).Take(4).SequenceEqual(m.MagicNumber))
                )
        };

        public static string GetMimeType(this byte[] ext)
        {
            if (ext == null)
                throw new ArgumentNullException(nameof(ext));

            MimeNameComparer cmp = new MimeNameComparer();
            List<MimeMarkers> tmp = _markers.Where(p => p.CheckMime(ext)).Distinct(cmp).ToList();

            if (tmp.Count > 1)
            {
                int maxH = tmp.Max(p => p.MagicNumber.Length);
                tmp = tmp.Where(p => p.MagicNumber.Length == maxH).ToList();
            }

            if (tmp.Count > 1)
            {
                int maxT = tmp.Max(p => p.Trailer?.Length ?? 0);
                tmp = tmp.Where(p => p.Trailer != null && p.Trailer.Length == maxT).ToList();
            }

            if (tmp.Count == 1)
                return tmp.First().Mime;
            else
                return "application/octet-stream";
        }

        public static bool IsImage(this byte[] ext)
        {
            return ext.GetMimeType().StartsWith("image/");
        }
        public static bool IsVideo(this byte[] ext)
        {
            return ext.GetMimeType().StartsWith("video/");
        }
        public static bool IsPdf(this byte[] ext)
        {
            return ext.GetMimeType() == "application/pdf";
        }
        public static bool IsUnknown(this byte[] ext)
        {
            return ext.GetMimeType() == "application/octet-stream";
        }
    }
}
