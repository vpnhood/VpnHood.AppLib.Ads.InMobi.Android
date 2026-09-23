using Com.Vpnhood.Inmobi.Ads;
using VpnHood.Core.Client.Device.Droid.Utils;
using VpnHood.Core.Toolkit.Utils;

namespace VpnHood.AppLib.Ads.InMobi.Android;

public class InMobiUtil
{
    private static readonly AsyncLock InitLock = new();
    public static bool IsInitialized { get; private set; }
    public static int RequiredAndroidVersion => 28;
    public static bool IsAndroidVersionSupported => OperatingSystem.IsAndroidVersionAtLeast(RequiredAndroidVersion);

    public static async Task Initialize(Activity activity, string accountId, bool isDebugMode, TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        // it looks like there is conflict between InMobi SDK and Xamarin.GooglePlayServices.Ads.Identifier 118.2.0
        // However we have decided to stop testing InMobi SDK in any android lower than 9.0
        if (!IsAndroidVersionSupported)
            throw new NotSupportedException("InMobi SDK requires Android 9.0 or higher.");

        using var lockAsync = await InitLock.LockAsync(cancellationToken);
        if (IsInitialized)
            return;

        // initialize
        var initTask = await AndroidUtil.RunOnUiThread(activity,
                () => InMobiAdServiceFactory.InitializeInMobi(activity, accountId, Java.Lang.Boolean.ValueOf(isDebugMode))!.AsTask())
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        // wait for completion
        await initTask
            .WaitAsync(timeout, cancellationToken)
            .ConfigureAwait(false);

        IsInitialized = true;
    }
}