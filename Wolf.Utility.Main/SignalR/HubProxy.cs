using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Wolf.Utility.Main.Extensions.Methods;
using Wolf.Utility.Main.Logging.Enum;
#pragma warning disable 4014

#pragma warning disable 1998

namespace Wolf.Utility.Main.SignalR
{
    /// <summary>
    /// Remember to call Init from the constructor.
    /// </summary>
    public abstract class HubProxy
    {
        public delegate void SetupHubConnectionDelegate(HubConnection connection);

        public delegate void ConnectionChangedDelegate(HubConnection connection, string hubName, EventArgs args);
            
        public event ConnectionChangedDelegate OnConnected;
        public event ConnectionChangedDelegate OnDisconnected;

        private Task ConnectedEventHandlerStatus { get; set; }
        private Task DisconnectedEventHandlerStatus { get; set; }

        private string HubNameWithDefault => string.IsNullOrEmpty(HubName) ? "Hub" : HubName;


        protected HubConnection Connection;
        protected string HubName { get; set; }

        public bool ShouldReconnect { get; set; }

        protected async Task Init(string baseAddress, SetupHubConnectionDelegate setup, string accessToken = "",
            bool shouldReconnect = true, string hubName = "")
        {
            HubName = hubName;
            ShouldReconnect = shouldReconnect;

            if(string.IsNullOrEmpty(accessToken))
                Connection = new HubConnectionBuilder().WithUrl($"{baseAddress}{hubName}").Build();
            else
                Connection = new HubConnectionBuilder().WithUrl($"{baseAddress}{hubName}", options =>
                {
                    async Task<string> OptionsAccessTokenProvider() => accessToken;

                    options.AccessTokenProvider = OptionsAccessTokenProvider;
                }).Build();
            
            Connection.Closed += Connection_Closed;

            setup.Invoke(Connection);

            await Connect();
        }

        protected async Task<bool> EnsureConnection(int timeout = 2500)
        {
            try
            {
                if (Connection.State != HubConnectionState.Connected)
                {
                    Logging.Logging.Log(LogType.Warning,
                        "Connection State is not Connected. Attempting to start Connection...");
                    await Connect(4);

                    if (Connection.State != HubConnectionState.Disconnected)
                        await Task.CompletedTask.WaitUntil(
                            () => Connection.State == HubConnectionState.Connected, timeout: timeout);

                    return true;
                }
            }
            catch (TimeoutException)
            {
                return false;
            }

            return true;
        }

        protected async Task Connection_Closed(Exception arg)
        {
            if (arg != null)
                Logging.Logging.Log(LogType.Exception,
                    $"Connection Closed due to an Exception -> {arg.GetType()} => {arg.Message}; Stacktrace => {arg.StackTrace}");
            if (ShouldReconnect)
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await Connect();
            }
        }

        protected async Task Connect()
        {
            AwaitConnectedState();
            Logging.Logging.Log(LogType.Information, $"Attempting to connect to {HubNameWithDefault} and awaiting response...");
            await Connection.StartAsync();
            Logging.Logging.Log(LogType.Information, $"Connected to {HubNameWithDefault}...");
        }

        protected Task Connect(int maxAttempts)
        {
            AwaitConnectedState();

            Logging.Logging.Log(LogType.Information,
                $"State of connection to {HubNameWithDefault} before attempting to connect: {Connection.State}");
            var attempts = 0;
            while (true)
            {
                Logging.Logging.Log(LogType.Information, $"Attempt to connect to {HubNameWithDefault} nr. {attempts + 1}");
                Task.Run(async () => await Connection.StartAsync());

                if (Connection.State == HubConnectionState.Disconnected)
                {
                    Logging.Logging.Log(LogType.Information, $"State of {HubNameWithDefault} Connection: {HubConnectionState.Disconnected}");
                    attempts += 1;
                    if (attempts == maxAttempts) return Task.CompletedTask;
                    continue;
                }

                Logging.Logging.Log(LogType.Information, $"State of {HubNameWithDefault} Connection: {Connection.State}");
                break;
            }

            return Task.CompletedTask;
        }

        private async Task AwaitConnectedState()
        {
            if (ConnectedEventHandlerStatus == null)
            {
                ConnectedEventHandlerStatus = Task.CompletedTask.WaitUntil(() => Connection.State == HubConnectionState.Connected);
                await ConnectedEventHandlerStatus;
                ConnectedEventHandlerStatus = null;

                OnConnected?.Invoke(Connection, HubNameWithDefault, EventArgs.Empty);

                AwaitDisconnectedState();
            }
        }

        private async Task AwaitDisconnectedState()
        {
            if (DisconnectedEventHandlerStatus == null)
            {
                DisconnectedEventHandlerStatus = Task.CompletedTask.WaitUntil(() => Connection.State == HubConnectionState.Disconnected);
                await DisconnectedEventHandlerStatus;
                DisconnectedEventHandlerStatus = null;

                OnDisconnected?.Invoke(Connection, HubNameWithDefault, EventArgs.Empty);

                AwaitConnectedState();
            }
        }

    }
}
    