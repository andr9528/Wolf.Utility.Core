using System.Threading.Tasks;
using ZXing.Mobile;

namespace Wolf.Utility.Main.Xamarin
{
    public interface IQrScanningService
    {
        Task<string> ScanAsync(MobileBarcodeScanningOptions options);
    }
}