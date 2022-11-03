package xyz.face.unity.android;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.ResolveInfo;
import android.content.pm.PackageManager;
import android.net.Uri;

import androidx.browser.customtabs.CustomTabsIntent;
import androidx.browser.customtabs.CustomTabsService;

import java.util.ArrayList;
import java.util.List;

public class BrowserView {
    
    public static void launchUrl(Activity context, String url) {
        Uri uri = Uri.parse(url);
        new CustomTabsIntent.Builder()
                            .build()
                            .launchUrl(context, uri);
    }
}