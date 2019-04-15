using ZXing;
using ZXing.Net.Mobile.Forms;

namespace BarcodeScanner
{
    internal interface IBarcodeScannerView
    {
        ZXingScannerView ScannerView { get; }

        bool HasResult { get; set; }

        Result Result { get; set; }

        void OnScanResult(Result result);

        void Initialize();

        void Destroy();
    }
}