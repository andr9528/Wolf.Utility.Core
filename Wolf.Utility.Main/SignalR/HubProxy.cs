using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Wolf.Utility.Main.Extensions.Methods;
using Wolf.Utility.Main.Logging.Enum;


// ReSharper disable MethodOverloadWithOptionalParameter
#pragma warning disable 4014
#pragma warning disable 1998

namespace Wolf.Utility.Main.SignalR
{
    /// <summary>
    /// Remember to call Init from the constructor.
    /// </summary>
    public abstract class HubProxy
    {
        private readonly bool logInEventAwaiting;

        public delegate void SetupHubConnectionDelegate(HubConnection connection);

        public delegate void ConnectionChangedDelegate(HubConnection connection, string hubName, EventArgs args);
            
        public event ConnectionChangedDelegate OnConnected;
        public event ConnectionChangedDelegate OnDisconnected;
        public event ConnectionChangedDelegate OnConnecting;

        public HubConnectionState ConnectionState => Connection.State;
        public string HubNameWithDefault => string.IsNullOrEmpty(HubName) ? "Hub" : HubName;

        protected HubConnection Connection;
        protected string HubName { get; set; }

        private Task ConnectedEventHandlerStatus { get; set; }
        private Task DisconnectedEventHandlerStatus { get; set; }
        private Task ConnectingEventHandlerStatus { get; set; }
        private int EventCheckFrequency { get; set; }

        public bool ShouldReconnect { get; set; }

        protected HubProxy(bool logInEventAwaiting = false)
        {
            if (logInEventAwaiting) Logging.Logging.Log(LogType.Information, $"Log Spam from Connection State Monitoring is Enabled...");

            this.logInEventAwaiting = logInEventAwaiting;
        }

        protected async Task Init(string baseAddress, string hubName, SetupHubConnectionDelegate setup, TimeSpan eventCheckFrequency,
            bool shouldReconnect = true, bool isAzureConnection = false)
        {
            HubName = hubName;
            ShouldReconnect = shouldReconnect;
            EventCheckFrequency = eventCheckFrequency.Milliseconds;

            Connection =  isAzureConnection ? await InitAzure(baseAddress, hubName) : InitSelf(baseAddress, hubName);
            
            Connection.Closed += Connection_Closed;

            setup.Invoke(Connection);

            await Connect();
        }

        /// <summary>
        /// Initializer for a connection to a self-hosted SignalR hub
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="hubName"></param>
        /// <returns></returns>
        private HubConnection InitSelf(string baseAddress, string hubName)
        {
            if (baseAddress.Last() != '/') baseAddress += "/";
            Logging.Logging.Log(LogType.Information, $"Creating connection to {hubName} at {baseAddress}{hubName}");
            
            return new HubConnectionBuilder().WithUrl($"{baseAddress}{hubName}").Build();
        }

        /// <summary>
        /// Initializer for a connection to an Azure hosted SignalR hub
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="hubName"></param>
        /// <returns></returns>
        private async Task<HubConnection> InitAzure(string baseAddress, string hubName)
        {
            NegotiateInfo info = null;

            try
            {
                var url = $"{baseAddress}/api/negotiate";
                Logging.Logging.Log(LogType.Information, $"Getting Negotiation Information from: {url}");
                var negotiate = await new HttpClient().GetStringAsync(url);
                info = JsonConvert.DeserializeObject<NegotiateInfo>(negotiate);
            }
            catch (HttpRequestException e)
            {
                Logging.Logging.Log(LogType.Exception, $"Something went wrong requesting the Negotiation Information; Exception message: {e.Message}");
                throw;
            }

            Logging.Logging.Log(LogType.Information, $"Creating connection to {hubName} at {info.Url}");

            return new HubConnectionBuilder().AddNewtonsoftJsonProtocol().WithUrl($"{info.Url}",
                options => { options.AccessTokenProvider = () => Task.FromResult(info.AccessToken);}).Build();
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

        public void Reconnect(int attempts = 1)
        {
            using (var unused = attempts == 1 ? Connect() : Connect(attempts))
            { }
        }

        protected async Task Connect()
        {
            AwaitConnectingState();

            Logging.Logging.Log(LogType.Information, $"Attempting to connect to {HubNameWithDefault} and awaiting response...");
            await Connection.StartAsync();
            Logging.Logging.Log(LogType.Information, $"Connected to {HubNameWithDefault}...");
        }

        protected Task Connect(int maxAttempts)
        {
            AwaitConnectingState();

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
                ConnectedEventHandlerStatus = Task.CompletedTask.WaitUntil(() => Connection.State == HubConnectionState.Connected, EventCheckFrequency, 
                    shouldLogInLoop: logInEventAwaiting, logMessage: () => $"Waiting for connection state to become {HubConnectionState.Connected}. Current state is: {Connection.State}");
                await ConnectedEventHandlerStatus;
                ConnectedEventHandlerStatus = null;

                Logging.Logging.Log(LogType.Event, $"{HubNameWithDefault}: Connection State changed to: {Connection.State}");
                OnConnected?.Invoke(Connection, HubNameWithDefault, EventArgs.Empty);

                AwaitDisconnectedState();
                AwaitConnectingState();
            }
        }

        private async Task AwaitDisconnectedState()
        {
            if (DisconnectedEventHandlerStatus == null)
            {
                DisconnectedEventHandlerStatus = Task.CompletedTask.WaitUntil(() => Connection.State == HubConnectionState.Disconnected, EventCheckFrequency,
                    shouldLogInLoop: logInEventAwaiting, logMessage: () => $"Waiting for connection state to become {HubConnectionState.Disconnected}. Current state is: {Connection.State}");
                await DisconnectedEventHandlerStatus;
                DisconnectedEventHandlerStatus = null;

                Logging.Logging.Log(LogType.Event, $"{HubNameWithDefault}: Connection State changed to: {Connection.State}");
                OnDisconnected?.Invoke(Connection, HubNameWithDefault, EventArgs.Empty);

                AwaitConnectedState();
                AwaitConnectingState();
            }
        }

        private async Task AwaitConnectingState()
        {
            if (ConnectingEventHandlerStatus == null)
            {
                ConnectingEventHandlerStatus = Task.CompletedTask.WaitUntil(
                    () => Connection.State == HubConnectionState.Connecting ||
                          Connection.State == HubConnectionState.Reconnecting, EventCheckFrequency,
                    shouldLogInLoop: logInEventAwaiting, logMessage: () => $"Waiting for connection state to become {HubConnectionState.Connecting}" +
                                                       $" or {HubConnectionState.Reconnecting}. Current state is: {Connection.State}");
                await ConnectingEventHandlerStatus;
                ConnectingEventHandlerStatus = null;

                Logging.Logging.Log(LogType.Event, $"{HubNameWithDefault}: Connection State changed to: {Connection.State}");
                OnConnecting?.Invoke(Connection, HubNameWithDefault, EventArgs.Empty);

                AwaitConnectedState();
                AwaitDisconnectedState();
            }
        }

    }
}
    