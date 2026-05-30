using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace DYS.JPay
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
             
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                if (CheckSelfPermission(Manifest.Permission.BluetoothConnect) != Permission.Granted)
                {
                    RequestPermissions(new string[] { Manifest.Permission.BluetoothConnect }, 1);
                }
            }
        }
    }
}
