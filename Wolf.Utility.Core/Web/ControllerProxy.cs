using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;

namespace Wolf.Utility.Core.Web
{
    public abstract class ControllerProxy
    {
        protected readonly RestClient client;
        protected readonly IHandler handler;

        /// <summary>
        /// For use with Web Api's
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="controller"></param>
        /// <param name="handler"></param>
        protected ControllerProxy(string baseAddress, string controller, IHandler handler = null)
        {
            client = new RestClient() { BaseUrl = new Uri($"{baseAddress}api/{controller}/") };
            this.handler = handler;
        }

        /// <summary>
        /// For use with Websites
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="controller"></param>
        protected ControllerProxy(string baseAddress, string controller = default) 
        {
            var uri = new Uri($"{baseAddress}");
            if (controller != default)
                uri = new Uri($"{baseAddress}/{controller}");
            client = new RestClient() { BaseUrl = uri };
        }


    }
}
