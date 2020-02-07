using System;
using System.Collections.Generic;
using System.Text;
using Wolf.Utility.Main.Xamarin.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(ServiceCollection))]
namespace Wolf.Utility.Main.Xamarin.Services
{
    class ServiceCollection : IServiceCollection
    {
        public IKeyboardService KeyboardService { get; private set; }

        public ServiceCollection()
        {
            
        }

        public void SetKeyboardService(IKeyboardService service)
        {
            if (KeyboardService == null) KeyboardService = service;
        }
    }
}
