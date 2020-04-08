using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace BarcodeScanner
{
    public class ContentPageBarcodeScannerService : ContentPage, IBarcodeScannerService, IBarcodeScannerView
    {
        private ZXingScannerView _scannerView;
        ZXingScannerView IBarcodeScannerView.ScannerView => _scannerView;

        private BarcodeScannerController _controller { get; }

        public ContentPageBarcodeScannerService()
        {
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

        protected virtual string TopText() => BarcodeScannerOptions.TopText;

        protected virtual string BottomText() => BarcodeScannerOptions.BottomText;

        string IBarcodeScannerView.TopText() => TopText();

        string IBarcodeScannerView.BottomText() => BottomText();

        protected virtual View GetScannerOverlay()
        {
            return _controller.DefaultOverlay;
        }

        protected virtual MobileBarcodeScanningOptions GetScanningOptions()
        {
            return BarcodeScannerOptions.DefaultScanningOptions;
        }

        protected override void OnAppearing() =>
            _controller.OnAppearing();

        protected override void OnDisappearing() =>
            _controller.OnDisappearing();

        private ContentPage GetCurrentContentPage()
        {
            var cp = GetCurrentPage();
            var mp = TryGetModalPage(cp);
            return mp ?? cp;
        }

        private ContentPage TryGetModalPage(ContentPage cp)
        {
            var mp = cp.Navigation.ModalStack.LastOrDefault();
            if (mp != null)
            {
                return GetCurrentPage(mp);
            }

            return null;
        }

        private ContentPage GetCurrentPage(Page page = null)
        {
            switch (page)
            {
                case ContentPage cp:
                    return cp;
                case TabbedPage tp:
                    return GetCurrentPage(tp.CurrentPage);
                case NavigationPage np:
                    return GetCurrentPage(np.CurrentPage);
                case CarouselPage carouselPage:
                    return GetCurrentPage(carouselPage.CurrentPage);
                case MasterDetailPage mdp:
                    mdp.IsPresented = false;
                    return GetCurrentPage(mdp.Detail);
                case Shell shell:
                    return GetCurrentPage((shell.CurrentItem.CurrentItem as IShellSectionController).PresentedPage);
                default:
                    // If we get some random Page Type
                    if (page != null)
                    {
                        Xamarin.Forms.Internals.Log.Warning("Warning", $"An Unknown Page type {page.GetType()} was found walk walking the Navigation Stack. This is not supported by the DialogService");
                        return null;
                    }

                    var mainPage = Application.Current?.MainPage;
                    if (mainPage is null)
                    {
                        return null;
                    }

                    return GetCurrentPage(mainPage);
            }
        }

        public void DoPush()
        {
            var currentPage = GetCurrentContentPage();
            currentPage.Navigation.PushModalAsync(this);
        }

        public void DoPop()
        {
            Navigation.PopModalAsync();
        }

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
