using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;

namespace Wolf.Utility.Core.Web
{
    /// <summary>
    /// TODO: Requires Reworking, to be functional again
    /// </summary>
    public abstract class ControllerProxy
    {
        protected readonly RestClient client;

        /// <summary>
        /// For use with connecting to Apis.
        /// Omit any '/', as they are included automatically.
        /// </summary>
        /// <param name="baseAddress">The base location for the api.</param>
        /// <param name="controllerSegments">Segments to append one after another, after 'api' in the URL, with a '/' between them</param>
        protected ControllerProxy(string baseAddress, params string[] controllerSegments)
        {
            var builder = new StringBuilder($"{baseAddress}/api/");
            foreach (string controllerSegment in controllerSegments)
                builder.Append($"{controllerSegment}/");
            var uri = new Uri(builder.ToString());
            client = new RestClient() { BaseUrl = uri, };
        }
    }
}
