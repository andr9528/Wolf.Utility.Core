using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Wolf.Utility.Main.Logging.Enum;

namespace Wolf.Utility.Main.SignalR
{
    public abstract class AdvancedHub<THub> : Hub where THub : Hub
    {
        protected readonly List<string> ConnectedIds = new List<string>();
        protected readonly IHubContext<THub> context;

        protected AdvancedHub(IHubContext<THub> context)
        {
            this.context = context;
        }

        public override Task OnConnectedAsync()
        {
            Logging.Logging.Log(LogType.Event, $"{nameof(THub)}: Client Connected with Id: {Context.ConnectionId}");
            ConnectedIds.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Logging.Logging.Log(LogType.Event, $"{nameof(THub)}: Client Disconnected with Id: {Context.ConnectionId}");
            if (exception != null)
                Logging.Logging.Log(LogType.Exception,
                    $"{nameof(THub)}: Connection with an Id of {Context.ConnectionId} Closed due to an Exception -> {exception.GetType()} => {exception.Message}; Stacktrace => {exception.StackTrace}");
            ConnectedIds.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
