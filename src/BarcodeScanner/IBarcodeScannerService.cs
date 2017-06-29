using System.Threading.Tasks;
using ZXing;

namespace BarcodeScanner
{
    public interface IBarcodeScannerService
    {
        Task<string> ReadBarcodeAsync();

        Task<Result> ReadBarcodeResultAsync();
    }
}