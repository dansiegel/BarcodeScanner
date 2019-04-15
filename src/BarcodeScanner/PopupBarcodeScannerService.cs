using System;
using System.Collections.Generic;
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
        public ZXingScannerView ScannerView { get; }

        public bool HasResult { get; set; }

        public Result Result { get; set; }

        protected IPopupNavigation PopupNavigation { get; }

        private ZXingDefaultOverlay DefaultOverlay { get; set; }

        public PopupBarcodeScannerService(IPopupNavigation popupNavigation)
        {
            PopupNavigation = popupNavigation;
            DefaultOverlay = new ZXingDefaultOverlay
            {
                TopText = TopText(),
                BottomText = BottomText(),
                ShowFlashButton = ScannerView.HasTorch,
                AutomationId = "zxingDefaultOverlay",
            };

            Padding = GetDefaultPadding();

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

        protected virtual bool ShouldCloseOnBackgroundTapped() => BarcodeScannerOptions.ShouldCloseOnBackgroundTapped;

        protected virtual string TopText() => BarcodeScannerOptions.TopText;

        protected virtual string BottomText() => BarcodeScannerOptions.BottomText;

        protected virtual View GetScannerOverlay()
        {
            DefaultOverlay = new ZXingDefaultOverlay
            {
                TopText = TopText(),
                BottomText = BottomText(),
                ShowFlashButton = ScannerView.HasTorch,
                AutomationId = "zxingDefaultOverlay",
            };

            return DefaultOverlay;
        }

        protected virtual Thickness GetDefaultPadding() =>
            new Thickness(60, 120);

        protected virtual MobileBarcodeScanningOptions GetScanningOptions()
        {
            return BarcodeScannerOptions.DefaultScanningOptions;
        }

        public async Task<string> ReadBarcodeAsync()
        {
            var result = await ReadBarcodeResultAsync();
            return result?.Text;
        }

        public async Task<string> ReadBarcodeAsync(params BarcodeFormat[] barcodeFormats)
        {
            var result = await ReadBarcodeResultAsync(barcodeFormats);
            return result?.Text;
        }

        public async Task<Result> ReadBarcodeResultAsync()
        {
            await PopupNavigation.PushAsync(this);
            await Task.Run(() => { while(!HasResult) { } });
            await PopupNavigation.RemovePageAsync(this);
            return Result;
        }

        public async Task<Result> ReadBarcodeResultAsync(params BarcodeFormat[] barcodeFormats)
        {
            var initialFormats = ScannerView.Options.PossibleFormats;
            ScannerView.Options.PossibleFormats = new List<BarcodeFormat>(barcodeFormats);
            var result = await ReadBarcodeResultAsync();
            ScannerView.Options.PossibleFormats = initialFormats;
            return result;
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

            if(DefaultOverlay != null)
            {
                DefaultOverlay.FlashButtonClicked += OnFlashButtonClicked;
            }
        }

        void IBarcodeScannerView.Destroy()
        {
            HasResult = true;
            ScannerView.IsScanning = false;
            ScannerView.OnScanResult -= OnScanResult;

            if (DefaultOverlay != null)
            {
                DefaultOverlay.FlashButtonClicked -= OnFlashButtonClicked;
            }
        }

        protected override bool OnBackgroundClicked()
        {
            if(ShouldCloseOnBackgroundTapped())
                HasResult = true;

            return base.OnBackgroundClicked();
        }

        private void OnFlashButtonClicked(object sender, EventArgs args)
        {
            ScannerView.IsTorchOn = !ScannerView.IsTorchOn;
        }
    }
}
