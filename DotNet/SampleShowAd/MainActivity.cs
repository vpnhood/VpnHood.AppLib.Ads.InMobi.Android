using VpnHood.AppLib.Ads.InMobi.Android;
using VpnHood.Core.Client.Device.Droid;
using VpnHood.Core.Client.Device.Droid.ActivityEvents;

namespace SampleShowAd;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : ActivityEvent
{
    private InMobiAdProvider? _inMobiAdProvider;
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        // ReSharper disable once AccessToStaticMemberViaDerivedType
        SetContentView(Resource.Layout.activity_main);
        _ = ShowAd();
    }

    private async Task ShowAd()
    {
        try {
            _inMobiAdProvider = InMobiAdProvider.Create(InMobiCredential.AccountId, InMobiCredential.PlacementId, TimeSpan.FromSeconds(5), true);
            await _inMobiAdProvider.LoadAd(new AndroidUiContext(this), CancellationToken.None);
            await _inMobiAdProvider.ShowAd(new AndroidUiContext(this), string.Empty, CancellationToken.None);
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
}