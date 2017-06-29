using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace BarcodeScanner
{
    public class PopupBarcodeScannerService : PopupPage, IBarcodeScannerService
    {
        protected ZXingScannerView scannerView { get; }

        private bool HasResult { get; set; }

        private Result Result { get; set; }

        public PopupBarcodeScannerService()
        {
            BackgroundClicked += (sender, e) => HasResult = true;

            Padding = GetDefaultPadding();

            scannerView = new ZXingScannerView
            {
                AutomationId = "zxingScannerView",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Options = GetScanningOptions(),
            };

            scannerView.OnScanResult += OnScanResult;

            var overlay = GetScannerOverlay();

            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            grid.Children.Add(scannerView);

            if(overlay != null)
            {
                grid.Children.Add(overlay);
            }

            // The root page of your application
            Content = grid;
        }

        protected virtual bool ShouldCloseOnBackgroundTapped() => true;

        protected virtual string TopText() => string.Empty;

        protected virtual string BottomText() => string.Empty;

        protected virtual View GetScannerOverlay()
        {
            var overlay = new ZXingDefaultOverlay
            {
                TopText = TopText(),
                BottomText = BottomText(),
                ShowFlashButton = scannerView.HasTorch,
                AutomationId = "zxingDefaultOverlay",
            };

            overlay.FlashButtonClicked += (sender, e) =>
            {
                scannerView.IsTorchOn = !scannerView.IsTorchOn;
            };

            return overlay;
        }

        protected virtual Thickness GetDefaultPadding() =>
            new Thickness(60, 120);

        protected virtual MobileBarcodeScanningOptions GetScanningOptions()
        {
            return new MobileBarcodeScanningOptions()
            {
                AutoRotate = false,
                TryHarder = true,
                UseNativeScanning = true,
                UseFrontCameraIfAvailable = false,
            };
        }

        public async Task<string> ReadBarcodeAsync()
        {
            await PopupNavigation.Instance.PushAsync(this);
            await Task.Run(() => { while(!HasResult) { } });
            await PopupNavigation.Instance.RemovePageAsync(this);
            return Result?.Text;
        }

        public async Task<Result> ReadBarcodeResultAsync()
        {
            await PopupNavigation.Instance.PushAsync(this);
            await Task.Run(() => { while(!HasResult) { } });
            await PopupNavigation.Instance.RemovePageAsync(this);
            return Result;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            HasResult = false;
            Result = null;
            scannerView.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            HasResult = true;
            scannerView.IsScanning = false;
        }

        private void OnScanResult(Result result)
        {
            Result = result;
            HasResult = true;
        }
    }
}
