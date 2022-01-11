using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Wpf.Core.Enums;

namespace Wolf.Utility.Core.Wpf.Core.Models
{
    public interface INavigationPageConfig
    {
        bool StartHidden { get; set; }
        NavigationLocation NavigationLocation { get; set; }
    }
}
