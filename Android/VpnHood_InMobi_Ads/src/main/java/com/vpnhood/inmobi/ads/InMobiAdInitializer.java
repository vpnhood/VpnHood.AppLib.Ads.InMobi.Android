package com.vpnhood.inmobi.ads;

import android.content.Context;
import android.util.Log;

import com.inmobi.sdk.InMobiSdk;

import org.json.JSONObject;

import java.util.concurrent.CompletableFuture;

class InMobiAdInitializer {
    private static final String TAG = "VpnHoodInMobi";

    public static CompletableFuture<Void> Initialize(Context context, String accountId, Boolean isDebugMode) {
        CompletableFuture<Void> task = new CompletableFuture<>();

        if (isDebugMode)
            InMobiSdk.setLogLevel(InMobiSdk.LogLevel.DEBUG);

        // No consent of our own: the SDK reads the person's answer that a consent platform stored under
        // IAB TCF (Google's UMP form stores one) and merges this object with it, where any claim of ours
        // could only contradict that answer.
        InMobiSdk.init(context, accountId, new JSONObject(), error -> {
            if (error != null) {
                Log.e(TAG, "InMobi Init failed -" + error.getMessage());
                task.completeExceptionally(error);
            } else {
                Log.d(TAG, "InMobi Init Successful");
                task.complete(null);
            }
        });

        return task;
    }
}
