using System;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace BarcodeScanner
{
    public class PopupBarcodeScannerService : PopupPage, IBarcodeScannerService, IBarcodeScannerView
    {
        private ZXingScannerView _scannerView;
        ZXingScannerView IBarcodeScannerView.ScannerView => _scannerView;

        private BarcodeScannerController _controller { get; }

        protected IPopupNavigation PopupNavigation { get; }


        public PopupBarcodeScannerService(IPopupNavigation popupNavigation)
        {
            PopupNavigation = popupNavigation;

            Padding = GetDefaultPadding();

            _scannerView = new ZXingScannerView
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

            grid.Children.Add(_scannerView);

            if(overlay != null)
            {
                grid.Children.Add(overlay);
            }

            _controller = new BarcodeScannerController(this);
            Content = grid;
        }

        protected virtual bool ShouldCloseOnBackgroundTapped() => BarcodeScannerOptions.ShouldCloseOnBackgroundTapped;

        protected virtual string TopText() => BarcodeScannerOptions.TopText;

        protected virtual string BottomText() => BarcodeScannerOptions.BottomText;

        string IBarcodeScannerView.TopText() => TopText();

        string IBarcodeScannerView.BottomText() => BottomText();

        protected virtual View GetScannerOverlay()
        {
            return _controller.DefaultOverlay;
        }

        protected virtual Thickness GetDefaultPadding() =>
            new Thickness(60, 120);

        protected virtual MobileBarcodeScanningOptions GetScanningOptions()
        {
            return BarcodeScannerOptions.DefaultScanningOptions;
        }

        protected override void OnAppearing() =>
            _controller.OnAppearing();

        protected override void OnDisappearing() =>
            _controller.OnDisappearing();

        protected override bool OnBackgroundClicked()
        {
            if (ShouldCloseOnBackgroundTapped())
                _controller.OnScanResult(null);

            return base.OnBackgroundClicked();
        }

        public void DoPush() =>
            PopupNavigation.PushAsync(this);

        public void DoPop() =>
            PopupNavigation.RemovePageAsync(this);

        public Task<string> ReadBarcodeAsync() =>
            _controller.ReadBarcodeAsync();

        public Task<string> ReadBarcodeAsync(params BarcodeFormat[] barcodeFormats) =>
            _controller.ReadBarcodeAsync(barcodeFormats);

        public Task<Result> ReadBarcodeResultAsync() =>
            _controller.ReadBarcodeResultAsync();

        public IObservable<Result> OnBarcodeResult() =>
            _controller.OnBarcodeResult();

        public Task<Result> ReadBarcodeResultAsync(params BarcodeFormat[] barcodeFormats) =>
            _controller.ReadBarcodeResultAsync(barcodeFormats);
    }
}
