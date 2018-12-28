package com.example.avbal.myapplication.Main;

import android.app.Activity;
import android.content.Context;
import android.net.wifi.WifiManager;

import com.example.avbal.myapplication.Controllers.StateController;

public class ApplicationContext
{
    // timeout for blocking operations in milliseconds
    public final static int Timeout = 1000;

    public final static String StateNone = "None";
    public final static String StateActive = "Active";
    public final static String StateOnline = "Online";
    public final static String StatePairing = "Pairing";
    public final static String StateAvailable = "Available";
    public final static String StateConnected = "Connected";
    public final static String StateDisconnected = "Disconnected";

    public final Activity MainActivity;
    public static ApplicationContext Instance;

    private final StateController stateController;

    public ApplicationContext(Activity activity)
    {
        Instance = this;
        MainActivity = activity;

        stateController = new StateController();
    }

    public void shutDown()
    {
        stateController.shutDown();
    }

    public WifiManager getWifiManager()
    {
        WifiManager wifiManager = (WifiManager) MainActivity.getApplicationContext()
                                                            .getSystemService(Context.WIFI_SERVICE);
        return wifiManager;
    }
}
