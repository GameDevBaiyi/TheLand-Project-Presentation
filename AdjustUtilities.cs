using com.adjust.sdk;

public static class AdjustUtilities
{
    public static void RecordReachLv5()
    {
        AdjustEvent adjustEvent = new AdjustEvent("5yh8iu");
        Adjust.trackEvent(adjustEvent);
    }

    public static void GateSuccess()
    {
        AdjustEvent adjustEvent = new AdjustEvent("vcd5r6");
        Adjust.trackEvent(adjustEvent);
    }

    public static void BundleMetaSuccess()
    {
        AdjustEvent adjustEvent = new AdjustEvent("ud9dpo");
        Adjust.trackEvent(adjustEvent);
    }

    public static void LoginWalletStart()
    {
        AdjustEvent adjustEvent = new AdjustEvent("b1nhlj");
        Adjust.trackEvent(adjustEvent);
    }

    public static void LoginWalletSuccess()
    {
        AdjustEvent adjustEvent = new AdjustEvent("gmsjka");
        Adjust.trackEvent(adjustEvent);
    }

    public static void AvatarStart()
    {
        AdjustEvent adjustEvent = new AdjustEvent("upua7d");
        Adjust.trackEvent(adjustEvent);
    }

    public static void AvatarSuccess()
    {
        AdjustEvent adjustEvent = new AdjustEvent("3nyy08");
        Adjust.trackEvent(adjustEvent);
    }

    public static void StartTutorial()
    {
        AdjustEvent adjustEvent = new AdjustEvent("6quzwf");
        Adjust.trackEvent(adjustEvent);
    }

    public static void RegisterWallet()
    {
        AdjustEvent adjustEvent = new AdjustEvent("abde9d");
        Adjust.trackEvent(adjustEvent);
    }

    public static void CompletePasscode()
    {
        AdjustEvent adjustEvent = new AdjustEvent("l3plgl");
        Adjust.trackEvent(adjustEvent);
    }
}