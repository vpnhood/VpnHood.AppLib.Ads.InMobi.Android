package com.vpnhood.inmobi.ads;

import android.content.Context;
import java.util.concurrent.CompletableFuture;

public interface IInMobiAdProvider {
    CompletableFuture<Void> LoadAd(Context context);

    // completes with whether the user clicked the ad
    CompletableFuture<Boolean> ShowAd(Context context);
}
