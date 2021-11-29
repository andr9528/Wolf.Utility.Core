using Wolf.Utility.Xamarin.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(ServiceCollection))]
namespace Wolf.Utility.Xamarin.Services
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
