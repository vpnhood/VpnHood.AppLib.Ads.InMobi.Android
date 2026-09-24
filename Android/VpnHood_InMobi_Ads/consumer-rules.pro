# .NET calls this wrapper through JNI, which R8 cannot see: keep its public surface as it is
-keep class com.vpnhood.inmobi.ads.** { public *; }
