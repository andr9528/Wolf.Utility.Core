using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Main.Xamarin.Services
{
    public interface IServiceCollection
    {
        IKeyboardService KeyboardService { get; }
        void SetKeyboardService(IKeyboardService service);
    }
}
