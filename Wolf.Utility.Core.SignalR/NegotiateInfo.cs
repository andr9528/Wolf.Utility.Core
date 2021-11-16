using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Wolf.Utility.Core.SignalR
{
    public class NegotiateInfo
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "accessToken")]
        public string AccessToken { get; set; }
    }
}
