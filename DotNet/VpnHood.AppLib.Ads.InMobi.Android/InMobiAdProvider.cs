using Com.Vpnhood.Inmobi.Ads;
using VpnHood.AppLib.Abstractions;
using VpnHood.AppLib.Abstractions.AdExceptions;
using VpnHood.Core.Client.Device.Droid;
using VpnHood.Core.Client.Device.Droid.Utils;
using VpnHood.Core.Client.Device.UiContexts;
using VpnHood.Core.Common.Exceptions;

namespace VpnHood.AppLib.Ads.InMobi.Android;

public class InMobiAdProvider(string accountId, string placementId, TimeSpan initializeTimeout, bool isDebugMode) 
    : IAppAdProvider
{
    private IInMobiAdProvider? _inMobiAdProvider;

    public string NetworkName => "InMobi";
    public AppAdType AdType => AppAdType.InterstitialAd;
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
            throw new AdException("MainActivity has been destroyed before loading the ad.");

        // reset the last loaded ad
        AdLoadedTime = null;

        // initialize
        await InMobiUtil.Initialize(activity, accountId, isDebugMode, initializeTimeout, cancellationToken);
        _inMobiAdProvider = InMobiAdServiceFactory.Create(Java.Lang.Long.ValueOf(placementId))
                             ?? throw new AdException($"The {AdType} ad is not initialized");

        // Load Ad
        var loadAdTask = await AndroidUtil.RunOnUiThread(activity, () =>  _inMobiAdProvider.LoadAd(activity)!.AsTask())
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        await loadAdTask
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        AdLoadedTime = DateTime.Now;
    }

    public async Task<ShowAdResult> ShowAd(IUiContext uiContext, string? customData, CancellationToken cancellationToken)
    {
        var appUiContext = (AndroidUiContext)uiContext;
        var activity = appUiContext.Activity;
        if (activity.IsDestroyed)
            throw new AdException("MainActivity has been destroyed before showing the ad.");

        try
        {
            if (AdLoadedTime == null || _inMobiAdProvider == null)
                throw new AdException($"The {AdType} has not been loaded.");

            // wait for show or dismiss
            var showAdTask = await AndroidUtil.RunOnUiThread(activity, () => _inMobiAdProvider.ShowAd(activity)!.AsTask())
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
            
            await showAdTask
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            return ShowAdResult.Closed;
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