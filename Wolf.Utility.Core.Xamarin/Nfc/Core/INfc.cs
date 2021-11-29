using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ultz.XNFC;
using Wolf.Utility.Xamarin.Nfc.Core.Enum;

namespace Wolf.Utility.Xamarin.Nfc.Core
{
    public interface INfc
    {
        Task StartListeningAsync(IEnumerable<ActionType> actions, IEnumerable<TechType> techs);
        Task StopListeningAsync();

        Task<bool> TryStartListeningAsync(IEnumerable<ActionType> actions, IEnumerable<TechType> techs);
        Task<bool> TryStopListeningAsync();

        /// <summary>Fires when a NFC tag is detected.</summary>
        event EventHandler<TagDetectedEventArgs> TagDetected;

        /// <summary>If the device's nfc reader is enabled.</summary>
        bool Enabled { get; }

        /// <summary>If the device has a nfc reader.</summary>
        bool Available { get; }

        /// <summary>If the device can scan for NFC tags.</summary>
        bool CanScan { get; }

        /// <summary>If we are currently scanning for NFC tags.</summary>
        bool Scanning { get; }

        /// <summary>Anything the NFC is associated with.</summary>
        object Association { get; set; }

        void Callback(object o);
    }
}