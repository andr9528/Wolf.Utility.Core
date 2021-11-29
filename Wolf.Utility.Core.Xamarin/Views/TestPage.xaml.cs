using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Wolf.Utility.Xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestPage : ContentPage
    {
        public TestPage(params View[] views)
        {
            InitializeComponent();

            foreach (var view in views)
            {
                TheStack.Children.Add(view);
            }
        }
    }
}