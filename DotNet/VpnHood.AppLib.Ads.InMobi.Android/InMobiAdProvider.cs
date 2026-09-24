using Com.Vpnhood.Inmobi.Ads;
using VpnHood.AppLib.Abstractions.Ads;
using VpnHood.AppLib.Abstractions.Ads.AdExceptions;
using VpnHood.Core.Client.Devices.Abstractions.UiContexts;
using VpnHood.Core.Client.Devices.Android;
using VpnHood.Core.Client.Devices.Android.Utils;

namespace VpnHood.AppLib.Ads.InMobi.Android;

public class InMobiAdProvider(string accountId, string placementId, TimeSpan initializeTimeout, bool isDebugMode) 
    : IAdProvider
{
    private IInMobiAdProvider? _inMobiAdProvider;

    public string NetworkName => "InMobi";
    public AdType AdType => AdType.InterstitialAd;
    public DateTime? AdLoadedTime { get; private set; }
    public TimeSpan AdLifeSpan { get; } = TimeSpan.FromMinutes(45);
    public static int RequiredAndroidVersion => InMobiUtil.RequiredAndroidVersion;
    public static bool IsAndroidVersionSupported => InMobiUtil.IsAndroidVersionSupported;

    public static InMobiAdProvider Create(string accountId, string placementId, TimeSpan initializeTimeout, bool isDebugMode)
    {
        var ret = new InMobiAdProvider(accountId, placementId, initializeTimeout, isDebugMode);
        return ret;
    }
    
    public async Task LoadAd(IUiContext uiContext, CancellationToken cancellationToken)
    {
        var appUiContext = (AndroidUiContext)uiContext;
        var activity = appUiContext.Activity;
        if (activity.IsDestroyed)
            throw new LoadAdException("MainActivity has been destroyed before loading the ad.");

        // reset the last loaded ad
        AdLoadedTime = null;
        _inMobiAdProvider = null;

        // initialize
        await InMobiUtil.Initialize(activity, accountId, isDebugMode, initializeTimeout, cancellationToken);
        var inMobiAdProvider = InMobiAdServiceFactory.Create(Java.Lang.Long.ValueOf(placementId))
                               ?? throw new LoadAdException($"The {AdType} ad is not initialized");

        // Load Ad
        var loadAdTask = await AndroidUtils.RunOnUiThread(activity, () =>
                (inMobiAdProvider.LoadAd(activity) ?? throw new LoadAdException("InMobi did not start loading the ad.")).AsTask())
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            await loadAdTask
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // no inventory is a no-fill, not a failure; the wrapper leads its message with InMobi's status code
            var message = $"InMobi ad failed to load. {ex.Message}";
            throw ex.Message.Contains("NO_FILL")
                ? new NoFillAdException(message, ex)
                : new LoadAdException(message, ex);
        }

        _inMobiAdProvider = inMobiAdProvider;

        // the app ages ads against UTC
        AdLoadedTime = DateTime.UtcNow;
    }

    public async Task<ShowAdResult> ShowAd(IUiContext uiContext, string? customData, CancellationToken cancellationToken)
    {
        var appUiContext = (AndroidUiContext)uiContext;
        var activity = appUiContext.Activity;
        if (activity.IsDestroyed)
            throw new ShowAdException("MainActivity has been destroyed before showing the ad.");

        var inMobiAdProvider = _inMobiAdProvider;
        try
        {
            if (AdLoadedTime == null || inMobiAdProvider == null)
                throw new ShowAdException($"The {AdType} has not been loaded.");

            // wait for show or dismiss
            var showAdTask = await AndroidUtils.RunOnUiThread(activity, () =>
                    (inMobiAdProvider.ShowAd(activity) ?? throw new ShowAdException("InMobi did not start showing the ad.")).AsTask())
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
            
            // the wrapper completes the show with whether the user clicked the ad
            var result = await showAdTask
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            return bool.TryParse(result?.ToString(), out var isClicked) && isClicked
                ? ShowAdResult.Clicked
                : ShowAdResult.Closed;
        }
        finally
        {
            _inMobiAdProvider = null;
            AdLoadedTime = null;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}