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

        protected ControllerProxy(string baseAddress, string controller, IHandler handler = null)
        {
            client = new RestClient() { BaseUrl = new Uri($"{baseAddress}api/{controller}/") };
            this.handler = handler;
        }


    }
}
