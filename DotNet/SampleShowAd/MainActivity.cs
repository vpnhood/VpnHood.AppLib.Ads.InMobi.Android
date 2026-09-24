using VpnHood.AppLib.Ads.InMobi.Android;
using VpnHood.Core.Client.Devices.Android;
using VpnHood.Core.Client.Devices.Android.ActivityEvents;

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
            _inMobiAdProvider = InMobiAdProvider.Create(ReadUserValue("account_id.txt"), ReadUserValue("placement_id.txt"), TimeSpan.FromSeconds(5), true);
            await _inMobiAdProvider.LoadAd(new AndroidUiContext(this), CancellationToken.None);
            var result = await _inMobiAdProvider.ShowAd(new AndroidUiContext(this), string.Empty, CancellationToken.None);
            Console.WriteLine($"InMobi ShowAd result: {result}");
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }

    // The ids live in the private .user folder beside this repo, one value per file
    // (.user/ad/InMobi); the csproj embeds each one by its file name.
    private static string ReadUserValue(string fileName)
    {
        using var stream = typeof(MainActivity).Assembly.GetManifestResourceStream(fileName)
            ?? throw new InvalidOperationException($"{fileName} is missing. Put it in .user/ad/InMobi beside this repo.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd().Trim();
    }
}