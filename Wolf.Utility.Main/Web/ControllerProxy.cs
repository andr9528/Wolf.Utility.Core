using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Wolf.Utility.Main.Web
{
    public abstract class ControllerProxy
    {
        protected readonly HttpClient client;

        protected ControllerProxy(string baseAddress)
        {
            client = new HttpClient() { BaseAddress = new Uri($"{baseAddress}api/") };
        }
    }
}
