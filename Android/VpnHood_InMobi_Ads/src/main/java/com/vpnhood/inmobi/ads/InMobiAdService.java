package com.vpnhood.inmobi.ads;

import android.content.Context;

import androidx.annotation.NonNull;
import com.inmobi.ads.AdMetaInfo;
import com.inmobi.ads.InMobiAdRequestStatus;
import com.inmobi.ads.InMobiInterstitial;
import com.inmobi.ads.listeners.InterstitialAdEventListener;

import java.util.Map;
import java.util.concurrent.CompletableFuture;

class InMobiAdService extends InterstitialAdEventListener implements IInMobiAdProvider {

    private InMobiInterstitial _interstitialAd;
    public final Long _placementId;
    public CompletableFuture<Void> _loadTask;
    public CompletableFuture<Boolean> _showTask;
    private boolean _isAdImpression = false;
    private boolean _isClicked = false;

    public InMobiAdService(Long placementId){
        _placementId = placementId;
    }

    @Override
    public CompletableFuture<Void> LoadAd(Context context) {
        _interstitialAd = new InMobiInterstitial(context, _placementId, this);
        _loadTask = new CompletableFuture<>();
        _interstitialAd.load();
        return _loadTask;
    }

    @Override
    public CompletableFuture<Boolean> ShowAd(Context context) {
        _showTask = new CompletableFuture<>();
        _interstitialAd.show();
        return _showTask;
    }

    @Override
    public void onAdLoadSucceeded(@NonNull InMobiInterstitial ad, @NonNull AdMetaInfo info) {
        _loadTask.complete(null);
    }

    @Override
    public void onAdLoadFailed(@NonNull InMobiInterstitial ad, @NonNull InMobiAdRequestStatus status) {
        // the status code leads the message: .NET receives only this text and tells a no-fill (NO_FILL) by it
        _loadTask.completeExceptionally(new Exception(status.getStatusCode().name() + ": " + status.getMessage()));
    }

    @Override
    public void onAdClicked(@NonNull InMobiInterstitial ad, Map<Object, Object> params) {
        _isClicked = true;
    }

    @Override
    public void onAdDisplayFailed(@NonNull InMobiInterstitial ad) {
        _showTask.completeExceptionally(new Exception("Ad display failed."));
    }

    @Override
    public void onAdDismissed(@NonNull InMobiInterstitial ad) {
        if (_isAdImpression || _isClicked)
            _showTask.complete(_isClicked);
        else
            _showTask.completeExceptionally(new Exception("Ad dismissed before impression."));
    }

    @Override
    public void onUserLeftApplication(@NonNull InMobiInterstitial ad) {
        // a tap that opened the ad's target outside the app: the ad stays up and is dismissed when the user returns
        _isClicked = true;
    }

    @Override
    public void onAdImpression(@NonNull InMobiInterstitial ad) {
        _isAdImpression = true;
    }
}
