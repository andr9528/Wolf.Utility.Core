using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Core.Extensions.Methods
{
    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this ServiceProvider provider) 
        {
            var type = typeof(T);
            var obj = provider.GetService(type);

            var element = (T)obj;
            return element;
        }
    }
}
