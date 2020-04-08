using ZXing;
using ZXing.Net.Mobile.Forms;

namespace BarcodeScanner
{
    internal interface IBarcodeScannerView
    {
        ZXingScannerView ScannerView { get; }

        void DoPush();

        void DoPop();

        string TopText();

        string BottomText();
    }
}