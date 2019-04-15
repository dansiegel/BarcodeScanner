using System.Collections.Generic;
using ZXing;
using ZXing.Mobile;

namespace BarcodeScanner
{
    public static class BarcodeScannerOptions
    {
        public static MobileBarcodeScanningOptions DefaultScanningOptions { get; set; } =
            new MobileBarcodeScanningOptions()
            {
                AutoRotate = false,
                TryHarder = true,
                UseNativeScanning = true,
                UseFrontCameraIfAvailable = false,
            };

        public static bool ShouldCloseOnBackgroundTapped { get; set; } = true;

        public static string TopText { get; set; }

        public static string BottomText { get; set; }

        public static void UpdatePossibleFormats(params BarcodeFormat[] possibleFormats) =>
            UpdatePossibleFormats(new List<BarcodeFormat>(possibleFormats));

        public static void UpdatePossibleFormats(IEnumerable<BarcodeFormat> possibleFormats) => 
            UpdatePossibleFormats(new List<BarcodeFormat>(possibleFormats));

        public static void UpdatePossibleFormats(List<BarcodeFormat> possibleFormats) => 
            DefaultScanningOptions.PossibleFormats = possibleFormats;
    }
}
