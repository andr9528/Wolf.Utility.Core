using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Wpf.Controls.Enums;

namespace Wolf.Utility.Core.Wpf.Controls.Model
{
    public interface INavigationPageConfig
    {
        bool StartHidden { get; set; }
        NavigationLocation NavigationLocation { get; set; }
    }
}
