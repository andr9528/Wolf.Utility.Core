using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

using Wolf.Utility.Core.Wpf.Extensions;

namespace Wolf.Utility.Core.Wpf.Controls.Model
{
    public class NavigationInfo
    {
        private readonly ImageSource? icon;
        private readonly Page content;
        private readonly DesiredLocation desired;
        private readonly bool hasIcon;

        public ImageSource? Icon => icon;
        public Page Content => content;
        public string Title => content.Title;
        public DesiredLocation Desired => desired;
        public bool HasIcon => HasIcon;

        public enum DesiredLocation { Null, AbsoluteStart, Start, Middle, End, AbsoluteEnd}
        public NavigationInfo(Page content, byte[]? icon = default, DesiredLocation desired = DesiredLocation.AbsoluteStart)
        {
            if (desired == DesiredLocation.Null) 
                throw new NullReferenceException($"{nameof(desired)} was set to null location which is invalid.");
            this.content = content;
            this.desired = desired;

            if (icon != null) 
            {
                this.icon = ImageConverter.ByteToImageSource(icon);
                hasIcon = true;
            }
            else hasIcon = false;
                
        }
    }
}
