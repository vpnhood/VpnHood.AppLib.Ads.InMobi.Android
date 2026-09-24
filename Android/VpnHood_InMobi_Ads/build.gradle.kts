plugins {
    alias(libs.plugins.android.library)
}

android {
    namespace = "com.vpnhood.inmobi.ads"
    // 36, not 37: AGP 9.4 cannot read the package.xml that the android-36.1 and -37 platforms here carry,
    // and the wrapper needs nothing newer (the .NET side targets 37 on its own)
    compileSdk = 36
    // the build-tools the .NET Android workload uses (AndroidSdkBuildToolsVersion), so one set is provisioned
    buildToolsVersion = "36.0.0"

    defaultConfig {
        minSdk = 24
        consumerProguardFiles("consumer-rules.pro")
    }

    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }
}

dependencies {
    // Compiled against, not shipped: the app supplies the SDK. The .NET package takes it from Maven
    // beside this wrapper, at the version in gradle/libs.versions.toml.
    compileOnly(libs.inmobi.ads.kotlin)
}
