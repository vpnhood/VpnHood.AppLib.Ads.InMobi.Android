# VpnHood.AppLib.Ads.InMobi.Android

InMobi interstitial ads for an Android app built on [VpnHood](https://github.com/vpnhood/VpnHood): an ad
provider that the app's ad service loads and shows like its other ad networks. The package holds a small
Java wrapper over the InMobi SDK and ships the SDK with everything it needs at run time, so the app
references nothing else for InMobi.

## Requirements

- .NET 11 for Android (`net11.0-android37.0`).
- Android 9 (API 28) or later on the device: on an older one the provider refuses to load.
- An InMobi account id and the placement id of an interstitial (a number), from your InMobi dashboard.

## Use

Add the provider to the list the app gets as `AppOptions.AdProviderItems`:

```csharp
using VpnHood.AppLib.Ads.InMobi.Android;
using VpnHood.AppLib.App.Services.Ads;

if (InMobiAdProvider.IsAndroidVersionSupported)
    adProviderItems.Add(new AppAdProviderItem {
        AdProvider = InMobiAdProvider.Create(accountId, placementId,
            initializeTimeout: TimeSpan.FromSeconds(5), isDebugMode: false)
    });
```

The first ad it loads starts the InMobi SDK, which has `initializeTimeout` to finish; `isDebugMode` turns
on the SDK's debug log.

## Privacy

The provider passes no consent of its own. The InMobi SDK reads the person's answer from a consent
platform that stores it under IAB TCF, such as Google's UMP form, which VpnHood's AdMob package shows
where a law requires consent. Where an app shows no such form, InMobi gets no consent.
