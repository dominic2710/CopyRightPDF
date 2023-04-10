using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.Runtime;

namespace CopyRightPDF.Viewer.Mobile;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
    : base(handle, ownership)
    {
        //var wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
        //var macAddress = wifiManager.Class.

        //ConnectivityManager cm = (ConnectivityManager)Android.App.Application.Context.GetSystemService(Context.ConnectivityService);
        //var activeNetwork = cm.GetAllNetworks();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
