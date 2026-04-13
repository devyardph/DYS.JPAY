using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace DYS.JPay
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

#if IOS
    if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
    {
        Padding = new Thickness(0, 0, 0, -50); // adjust value as needed
    }
#endif
        }

    }
}

