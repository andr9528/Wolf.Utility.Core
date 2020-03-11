using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;

namespace Wolf.Utility.Main.Extensions
{
    public static class HubRouteBuilderExtensions
    {
        public static void MabHub<THub>(this HubRouteBuilder builder, PathString path, THub hub) where THub : Hub
        {
            builder.MapHub<THub>(path);
        }

        public static void MapHub<THub>(this HubRouteBuilder builder, PathString path,
            Action<HttpConnectionDispatcherOptions> configureOptions, THub hub) where THub : Hub
        {
            builder.MapHub<THub>(path, configureOptions);
        }


    }
}
