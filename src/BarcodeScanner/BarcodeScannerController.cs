using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace BarcodeScanner
{
    internal class BarcodeScannerController : IBarcodeScannerService
    {
        private IBarcodeScannerView _scannerView { get; }
        private Subject<Result> _observableResult = new Subject<Result>();
        private Action _onCompletedOrResult = null;
        private Result Result;

        public BarcodeScannerController(IBarcodeScannerView scannerView)
        {
            _scannerView = scannerView;
            DefaultOverlay = new ZXingDefaultOverlay
            {
                TopText = _scannerView.TopText(),
                BottomText = _scannerView.BottomText(),
                ShowFlashButton = _scannerView.ScannerView.HasTorch,
                AutomationId = "zxingDefaultOverlay",
            };
        }

        public ZXingDefaultOverlay DefaultOverlay { get; }

        public IObservable<Result> OnBarcodeResult()
        {
            ReadBarcodeInternal(r => { }, e =>
            {
                _observableResult.OnError(e);
            });
            return _observableResult;
        }

        public Task<string> ReadBarcodeAsync()
        {
            var tcs = new TaskCompletionSource<string>();
            ReadBarcodeInternal(r => tcs.SetResult(r.Text), e => tcs.SetException(e));
            return tcs.Task;
        }

        public Task<string> ReadBarcodeAsync(params BarcodeFormat[] barcodeFormats)
        {
            var initialFormats = _scannerView.ScannerView.Options.PossibleFormats;
            _scannerView.ScannerView.Options.PossibleFormats = new List<BarcodeFormat>(barcodeFormats);
            var tcs = new TaskCompletionSource<string>();
            ReadBarcodeInternal(r =>
            {
                _scannerView.ScannerView.Options.PossibleFormats = initialFormats;
                tcs.SetResult(r.Text);
            }, e => tcs.SetException(e));
            return tcs.Task;
        }

        public Task<Result> ReadBarcodeResultAsync()
        {
            var tcs = new TaskCompletionSource<Result>();
            ReadBarcodeInternal(r => tcs.SetResult(r), e => tcs.SetException(e));
            return tcs.Task;
        }

        public Task<Result> ReadBarcodeResultAsync(params BarcodeFormat[] barcodeFormats)
        {
            var initialFormats = _scannerView.ScannerView.Options.PossibleFormats;
            _scannerView.ScannerView.Options.PossibleFormats = new List<BarcodeFormat>(barcodeFormats);
            var tcs = new TaskCompletionSource<Result>();
            ReadBarcodeInternal(r =>
            {
                _scannerView.ScannerView.Options.PossibleFormats = initialFormats;
                tcs.SetResult(r);
            }, e => tcs.SetException(e));
            return tcs.Task;
        }

        public void OnScanResult(Result result)
        {
            Result = result ?? new Result(string.Empty, Array.Empty<byte>(), Array.Empty<ResultPoint>(), BarcodeFormat.All_1D);
            _observableResult.OnNext(Result);
            _onCompletedOrResult?.Invoke();
        }

        private void ReadBarcodeInternal(Action<Result> onResult, Action<Exception> onError)
        {
            try
            {
                _scannerView.DoPush();
                _onCompletedOrResult = HasResultChanged;
                void HasResultChanged()
                {
                    _onCompletedOrResult = null;
                    _scannerView.DoPop();
                    onResult.Invoke(Result);
                }
            }
            catch (Exception e)
            {
                onError(e);
            }
        }

        public void OnAppearing()
        {
            Result = null;
            _scannerView.ScannerView.IsScanning = true;
            _scannerView.ScannerView.OnScanResult += OnScanResult;
            DefaultOverlay.FlashButtonClicked += OnFlashButtonClicked;
        }

        public void OnDisappearing()
        {
            _scannerView.ScannerView.IsScanning = false;
            _scannerView.ScannerView.OnScanResult -= OnScanResult;
            DefaultOverlay.FlashButtonClicked -= OnFlashButtonClicked;
        }

        private void OnFlashButtonClicked(object sender, EventArgs args)
        {
            _scannerView.ScannerView.IsTorchOn = !_scannerView.ScannerView.IsTorchOn;
        }
    }
}
