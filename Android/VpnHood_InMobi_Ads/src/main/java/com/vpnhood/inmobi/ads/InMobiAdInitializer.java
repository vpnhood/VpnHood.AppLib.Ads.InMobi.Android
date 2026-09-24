package com.vpnhood.inmobi.ads;

import android.content.Context;
import android.util.Log;

import androidx.annotation.NonNull;

import com.inmobi.sdk.InMobiSdk;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.concurrent.CompletableFuture;

class InMobiAdInitializer {
    private static final String TAG = "VpnHoodInMobi";

    public static CompletableFuture<Void> Initialize(Context context, String accountId, Boolean isDebugMode) {
        CompletableFuture<Void> task = new CompletableFuture<>();

        JSONObject consentObject = getJsonObject();

        if (isDebugMode)
            InMobiSdk.setLogLevel(InMobiSdk.LogLevel.DEBUG);

        InMobiSdk.init(context, accountId, consentObject, error -> {
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

    private static @NonNull JSONObject getJsonObject() {
        try {
            JSONObject consentObject = new JSONObject();
            consentObject.put(InMobiSdk.IM_GDPR_CONSENT_AVAILABLE, true);
            consentObject.put("gdpr", "0");
            consentObject.put(InMobiSdk.IM_GDPR_CONSENT_IAB, true);
            return consentObject;
        } catch (JSONException e) {
            throw new RuntimeException(e.getMessage());
        }
    }

}
