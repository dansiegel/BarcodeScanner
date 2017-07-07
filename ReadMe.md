# Barcode Scanning Service

I have simply found that I'm relying on Barcode Scanning so often that it gets old having needing to implement barcode scanning over and over. This library aims to reduce your effort while developing a new app while providing you a framework that provides for ease in testing.

[![ScannerNuGetShield]][ScannerNuGet]

## License &amp; Support

Found a bug, or have an idea to make this library better please feel free to create an [issue][issues].

This library is distributed with an MIT License. You can make any modifications you require or patent any software using this library. If this library was helpful for your project please send me a tweet and remember I accept donations to help fund my various open source projects.

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.me/dansiegel)

## Getting Started

This library has no direct dependencies on any MVVM Framework or DI container and as such can be used with ease regardless of the Framework or DI Container you use. The sample code is based on Prism with DryIoc. Note that the `PopupBarcodeScannerService` does have a dependency on the `IPopupNavigation` service. you will need to be sure to register the `PopupNavigation.Instance` with your DI container.

```cs
public void RegisterTypes()
{
    // Uses a Popup Page to contain the Scanner
    Container.Register<IBarcodeScannerService, PopupBarcodeScannerService>();
    Container.UseInstance<IPopupNavigation>(PopupNavigation.Instance);

    // Uses a Content Page to contain the Scanner
    Container.Register<IBarcodeScannerService, ContentPageBarcodeScannerService>();
}

public class ViewAViewModel
{
    private IBarcodeScannerService _barcodeScanner { get; }

    public ViewAViewModel(IBarcodeScannerService barcodeScanner)
    {
        _barcodeScanner = barcodeScanner;
        ScanBarcodeCommand = new DelegateCommand(OnScanBarcodeCommandExecuted);
    }

    public DelegateCommand ScanBarcodeCommand { get; }

    private async void OnScanBarcodeCommandExecuted()
    {
        // Returns the string value of the Barcode
        string barcodeValue = await _barcodeScanner.ReadBarcodeAsync();

        // Returns the ZXing.NET Barcode Scan Result
        Result result = await _barcodeScanncer.ReadBarcodeResultAsync();
    }
}
```

### Customization

Both the `ContentPageBarcodeScannerService` and the `PopupBarcodeScannerService` are designed to allow you to customize the look and feel without having to worry about hooking into the Scan Result event.

```cs
public class MyBarcodeScannerService : PopupBarcodeScannerService
{
    public MyBarcodeScannerService()
        : base()
    {
        // You can access and set any properties of the ZXing Scanner View
        scannerView.HorizontalOptions = LayoutOptions.Center;
    }

    // The Top and Bottom Text are only used if you are using the Default Scanner Overlay
    protected override string TopText() => "Hello Scanner";

    protected override string BottomText() => "Point this at a barcode";

    // You can add any View that you'd like for the Overlay
    protected override View GetScannerOverlay() => new View();

    // You can choose to customize the Scanning Options such as below where we limit the scanning
    // to look for Code 128
    protected override MobileBarcodeScanningOptions GetScanningOptions()
    {
        var options = base.GetScanningOptions();
        options.PossibleFormats = new List<BarcodeFormat>()
                {
                    BarcodeFormat.CODE_128
                }
        return options;
    }
}
```

## Platform Setup

This library has no direct requirement to be configured on any platform. However it does depend on both [Rg.Plugins.Popup][RgPopup] and [ZXing.Net.Mobile][ZXingNet] which do require some initialization. *NOTE* Rg.Plugins.Popup requires no initialization on [Android or iOS][RgInitialization].

### Android

#### AssemblyInfo.cs

```cs
[assembly: UsesPermission(Android.Manifest.Permission.Flashlight)]
```

#### AndroidManifest.xml

```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.FLASHLIGHT" />
```

#### MainActivity.cs

```cs
public class MainActivity : FormsAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        TabLayoutResource = Resource.Layout.Tabbar;
        ToolbarResource = Resource.Layout.Toolbar;

        base.OnCreate(savedInstanceState);

        global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
        global::ZXing.Net.Mobile.Forms.Android.Platform.Init();

        LoadApplication(new App());
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
                                                    Permission[] grantResults)
    {
        global::ZXing.Net.Mobile
                         .Android
                         .PermissionsHandler
                         .OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}
```

### iOS

#### AppDelegate.cs

```cs
public partial class AppDelegate : FormsApplicationDelegate
{
    public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
    {
        global::Xamarin.Forms.Forms.Init();
        global::ZXing.Net.Mobile.Forms.iOS.Platform.Init();

        LoadApplication(new App());

        return base.FinishedLaunching(uiApplication, launchOptions);
    }
}
```

#### Info.plist

```cs
<key>NSCameraUsageDescription</key>
<string>Add your Camera Use Description here</string>
```

[issues]: https://github.com/dansiegel/BarcodeScanner/issues
[RgPopup]: https://github.com/rotorgames/Rg.Plugins.Popup
[ZXingNet]: https://github.com/Redth/ZXing.Net.Mobile
[RgInitialization]: https://github.com/rotorgames/Rg.Plugins.Popup#initialize
[ScannerNuGetShield]: https://img.shields.io/nuget/vpre/BarcodeScanning.Service.svg
[ScannerNuGet]: https://www.nuget.org/packages/BarcodeScanning.Service