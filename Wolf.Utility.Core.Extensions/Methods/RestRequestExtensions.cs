using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wolf.Utility.Core.Extensions.Methods
{
    public static class RestRequestExtensions
    {
        public static IRestRequest AddQueryParametersFromObject<T>(this IRestRequest request, T input,
            IEnumerable<string> propertyNames) where T : class
        {
            var properties = typeof(T).GetProperties().Where(x => propertyNames.Any(z => x.Name == z)).ToList();

            return properties.Aggregate(request, (current, info) => current.AddQueryParameter(info.Name, info.GetValue(input).ToString()));
        }

        public static IRestRequest AddQueryParametersFromObject<T>(this IRestRequest request, T input, 
            params string[] propertyNames) where T : class
        {
            return AddQueryParametersFromObject(request, input, propertyNames.ToList());
        }
    }
}
