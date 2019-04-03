using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace AsIKnow.WebHelpers
{
    public static class DataHelpersExtensions
    {
        public static PaginatedListBuilder<Q,T> PaginatedResponse<Q, T>(this HttpRequest request, IEnumerable<Q> items, Func<Q, T> transform, int defaultPerPage = 15, int defaultPage = 1)
        {
            return PaginatedListBuilder<Q, T>.FormRequest(request, items, defaultPerPage, defaultPage, transform);
        }
        public static PaginatedListBuilder<T, T> PaginatedResponse<T>(this HttpRequest request, IEnumerable<T> items, int defaultPerPage = 15, int defaultPage = 1)
        {
            return PaginatedListBuilder<T, T>.FormRequest(request, items, defaultPerPage, defaultPage, p=>p);
        }
    }
}
