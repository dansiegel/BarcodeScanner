using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace BarcodeScanner
{
    public class ContentPageBarcodeScannerService : ContentPage, IBarcodeScannerService, IBarcodeScannerView
    {
        public ZXingScannerView ScannerView { get; }

        public bool HasResult { get; set; }

        public Result Result { get; set; }

        public ContentPageBarcodeScannerService()
        {
            ScannerView = new ZXingScannerView
            {
                AutomationId = "zxingScannerView",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Options = GetScanningOptions(),
            };

            var overlay = GetScannerOverlay();

            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            grid.Children.Add(ScannerView);

            if(overlay != null)
            {
                grid.Children.Add(overlay);
            }

            // The root page of your application
            Content = grid;
        }

        public async Task<string> ReadBarcodeAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(this);
            await Task.Run(() => { while (!HasResult) { } });
            Application.Current.MainPage.Navigation.RemovePage(this);
            return Result?.Text;
        }

        public async Task<Result> ReadBarcodeResultAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(this);
            await Task.Run(() => { while (!HasResult) { } });
            Application.Current.MainPage.Navigation.RemovePage(this);
            return Result;
        }

        protected virtual string TopText() => BarcodeScannerOptions.TopText;

        protected virtual string BottomText() => BarcodeScannerOptions.BottomText;

        protected virtual View GetScannerOverlay()
        {
            var overlay = new ZXingDefaultOverlay
            {
                TopText = TopText(),
                BottomText = BottomText(),
                ShowFlashButton = ScannerView.HasTorch,
                AutomationId = "zxingDefaultOverlay",
            };

            overlay.FlashButtonClicked += (sender, e) =>
            {
                ScannerView.IsTorchOn = !ScannerView.IsTorchOn;
            };

            return overlay;
        }

        protected virtual MobileBarcodeScanningOptions GetScanningOptions()
        {
            return BarcodeScannerOptions.DefaultScanningOptions;
        }

        protected override void OnAppearing()
        {
            ((IBarcodeScannerView)this).Initialize();
        }

        protected override void OnDisappearing()
        {
            ((IBarcodeScannerView)this).Destroy();
        }

        public void OnScanResult(Result result)
        {
            Result = result;
            HasResult = true;
        }

        void IBarcodeScannerView.Initialize()
        {
            HasResult = false;
            Result = null;
            ScannerView.IsScanning = true;
            ScannerView.OnScanResult += OnScanResult;
        }

        void IBarcodeScannerView.Destroy()
        {
            HasResult = true;
            ScannerView.IsScanning = false;
            ScannerView.OnScanResult -= OnScanResult;
        }
    }
}
