using System;

using Wolf.Utility.Core.Deprecated.Logging;
using Wolf.Utility.Core.Deprecated.Logging.Enum;
using Wolf.Utility.Xamarin.Nfc.Core;

namespace Wolf.Utility.Xamarin.Nfc.Implementation
{
    public class GlobalNfc
    {
        public static INfc Nfc { get; private set; }

        public static void GlobalCallback(object obj)
        {
            if (Nfc == null)
                throw new NullReferenceException(
                    $"{nameof(Nfc)} was null. Remember to call Init, with an instance of INfc gotten from Dependency Injection, at the start of the program.");

            Logging.Log(LogType.Information, $"Global Callback Called");

            Nfc.Callback(obj);
        }

        public static void Init(INfc nfc)
        {
            if (Nfc == null)
                Nfc = nfc ?? throw new ArgumentNullException(nameof(nfc));
        }
    }
}