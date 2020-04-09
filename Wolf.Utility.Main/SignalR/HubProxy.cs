using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Wolf.Utility.Main.Extensions;
using Wolf.Utility.Main.Extensions.Methods;
using Wolf.Utility.Main.Logging.Enum;

namespace Wolf.Utility.Main.SignalR
{
    /// <summary>
    /// Remember to call Init from the constructor.
    /// </summary>
    public abstract class HubProxy
    {
        public delegate void SetupHubConnectionDelegate(HubConnection connection);
        
        protected HubConnection Connection;
        protected string HubName { get; private set; }

        public bool ShouldReconnect { get; set; }

        protected async Task Init(string baseAddress, string hubName, SetupHubConnectionDelegate setup, bool shouldReconnect = true)
        {
            HubName = hubName;
            ShouldReconnect = shouldReconnect;

            Connection = new HubConnectionBuilder().WithUrl($"{baseAddress}{hubName}").Build();
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
                        $"Connection State is not Connected. Attempting to start Connection...");
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
            Logging.Logging.Log(LogType.Information, $"Attempting to connect to {HubName} and awaiting response...");
            await Connection.StartAsync();
            Logging.Logging.Log(LogType.Information, $"Connected to {HubName}...");
        }

        protected Task Connect(int maxAttempts)
        {
            Logging.Logging.Log(LogType.Information,
                $"State of connection to {HubName} before attempting to connect: {Connection.State}");
            var attempts = 0;
            while (true)
            {
                Logging.Logging.Log(LogType.Information, $"Attempt to connect to {HubName} nr. {attempts + 1}");
                Task.Run(async () => await Connection.StartAsync());

                if (Connection.State == HubConnectionState.Disconnected)
                {
                    Logging.Logging.Log(LogType.Information, $"State of {HubName} Connection: {HubConnectionState.Disconnected}");
                    attempts += 1;
                    if (attempts == maxAttempts) return Task.CompletedTask;
                    continue;
                }

                Logging.Logging.Log(LogType.Information, $"State of {HubName} Connection: {Connection.State}");
                break;
            }

            return Task.CompletedTask;
        }

    }
}
    