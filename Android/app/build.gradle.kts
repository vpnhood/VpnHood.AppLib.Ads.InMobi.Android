plugins {
    alias(libs.plugins.android.application)
}

// The ad ids, one value per file in the private .user folder beside this repo (.user/ad/InMobi),
// never in git. A missing file gives an empty value rather than a failed build: the .NET build
// configures this module too, on machines without .user.
val inMobiUserDir = rootDir.resolve("../../.user/ad/InMobi")
fun inMobiUserValue(fileName: String): String =
    inMobiUserDir.resolve(fileName).takeIf { it.exists() }?.readText()?.trim() ?: ""

android {
    namespace = "com.example.myinmobiads"
    compileSdk = 36
    buildToolsVersion = "36.0.0"

    defaultConfig {
        applicationId = "com.example.myinmobiads"
        minSdk = 24
        targetSdk = 36
        versionCode = 1
        versionName = "1.0"
        buildConfigField("String", "INMOBI_ACCOUNT_ID", "\"${inMobiUserValue("account_id.txt")}\"")
        buildConfigField("long", "INMOBI_PLACEMENT_ID", "${inMobiUserValue("placement_id.txt").ifEmpty { "0" }}L")
    }

    buildFeatures {
        buildConfig = true
    }

    buildTypes {
        release {
            isMinifyEnabled = false
            proguardFiles(getDefaultProguardFile("proguard-android-optimize.txt"), "proguard-rules.pro")
        }
    }

    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }
}

dependencies {
    implementation(project(":VpnHood_InMobi_Ads"))
    // the wrapper only compiles against the SDK; the app ships it
    implementation(libs.inmobi.ads.kotlin)
    implementation(libs.appcompat)
    implementation(libs.material)
    implementation(libs.activity)
    implementation(libs.constraintlayout)
}
