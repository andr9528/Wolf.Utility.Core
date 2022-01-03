using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

using Wolf.Utility.Core.Wpf.Controls.Enums;
using Wolf.Utility.Core.Wpf.Extensions;

namespace Wolf.Utility.Core.Wpf.Controls.Model
{
    public class NavigationInfo
    {
        private readonly ImageSource? icon;
        private readonly Page content;
        private readonly NavigationOrder desired;
        private readonly bool hasIcon = false;

        public ImageSource? Icon => icon;
        public Page Content => content;
        public string Title => content.Title;
        public NavigationOrder Desired => desired;
        public bool HasIcon => hasIcon;

        public NavigationInfo(Page content, byte[]? icon = default, NavigationOrder desired = NavigationOrder.AbsoluteStart)
        {
            if (desired == NavigationOrder.Null) 
                throw new NullReferenceException($"{nameof(desired)} was set to null location which is invalid.");
            this.content = content;
            this.desired = desired;

            if (icon != null) 
            {
                this.icon = ImageConverter.ByteToImageSource(icon);
                hasIcon = true;
            }                
        }
    }
}
