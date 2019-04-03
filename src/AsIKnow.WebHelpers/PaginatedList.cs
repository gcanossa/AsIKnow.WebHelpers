using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsIKnow.WebHelpers
{
    public abstract class PaginatedList<T>
    {
        #region nested types

        public class LinksObj
        {
            public Uri First { get; set; }
            public Uri Last { get; set; }
            public Uri Prev { get; set; }
            public Uri Next { get; set; }
        }
        public class MetaObj
        {
            public int CurrentPage { get; set; }
            public int From { get; set; }
            public int LastPage { get; set; }
            public string Path { get; set; }
            public int PerPage { get; set; }
            public int To { get; set; }
            public int Total { get; set; }
        }

        #endregion

        public List<T> Data { get; protected set; }
        public LinksObj Links { get; protected set; }
        public MetaObj Meta { get; protected set; }
    }

    public class PaginatedListBuilder<Q, T> : PaginatedList<T>
    {
        public static PaginatedListBuilder<Q,T> FormRequest(HttpRequest request, IEnumerable<Q> items, int defaultPerPage = 15, int defaultPage = 1, Func<Q, T> transform = null)
        {
            return FormRequest(request, items?.AsQueryable(), defaultPerPage, defaultPage, transform);
        }

        public static PaginatedListBuilder<Q,T> FormRequest(HttpRequest request, IQueryable<Q> items, int defaultPerPage = 15, int defaultPage = 1, Func<Q, T> transform = null)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            items = items ?? throw new ArgumentNullException(nameof(items));

            return new PaginatedListBuilder<Q,T>(
                new Uri(request.GetEncodedUrl()),
                items,
                !request.Query.ContainsKey("limit") ? defaultPerPage : Convert.ToInt32(request.Query["limit"].FirstOrDefault() ?? defaultPerPage.ToString()),
                !request.Query.ContainsKey("page") ? defaultPage : Convert.ToInt32(request.Query["page"].FirstOrDefault() ?? defaultPage.ToString()),
                transform
            );
        }

        public PaginatedListBuilder(Uri baseUri, IEnumerable<Q> items, int perPage, int page, Func<Q, T> transform)
            :this(baseUri, items?.AsQueryable(), perPage, page, transform)
        {}

        public PaginatedListBuilder(Uri baseUri, IQueryable<Q> items, int perPage, int page, Func<Q, T> transform)
        {
            UriBuilder b = new UriBuilder(baseUri);
            b.Query = string.Join("&", b.Query.Split('&').Where(p => !p.Contains("page=")));
            baseUri = b.Uri;
            int total = items.Count();
            int numberOfPages = total / perPage + (total % perPage > 0 ? 1 : 0);
            Meta = new MetaObj()
            {
                CurrentPage = page,
                From = (page - 1) * perPage + 1,
                To = page * perPage - (page == numberOfPages ? perPage - total % perPage : 0),
                LastPage = numberOfPages,
                PerPage = perPage,
                Total = total,
                Path = baseUri.ToString()
            };

            Links = new LinksObj
            {
                First = new Uri(QueryHelpers.AddQueryString(baseUri.ToString(), "page", "1")),
                Last = new Uri(QueryHelpers.AddQueryString(baseUri.ToString(), "page", $"{numberOfPages}")),
                Prev = page == 1 ? null : new Uri(QueryHelpers.AddQueryString(baseUri.ToString(), "page", $"{page - 1}")),
                Next = page == numberOfPages ? null : new Uri(QueryHelpers.AddQueryString(baseUri.ToString(), "page", $"{page + 1}"))
            };

            Data = items.Skip((page - 1) * perPage).Take(perPage).Select(transform).ToList();
        }
        
        public List<T> Data { get; protected set; }
        public LinksObj Links { get; protected set; }
        public MetaObj Meta { get; protected set; }
    }
}
