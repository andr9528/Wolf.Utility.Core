using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Main.Xamarin
{
    public interface IKeyboardService
    {
        bool IsKeyboardShown { get; }
        void HideKeyboard();
    }
}
