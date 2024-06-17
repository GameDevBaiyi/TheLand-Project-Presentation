using UnityEngine;

public class TalkingDataManager : MonoBehaviour
{
    private void Awake()
    {
        if (!GameSetting.Instance.IsPhoneAndPrdEntryUrl) return;
        TalkingDataSDK.BackgroundSessionEnabled();
        string channelId = null;
#if UNITY_ANDROID
        channelId = "GooglePlay";
#elif UNITY_IOS
        channelId = "AppStore";
#endif
        TalkingDataSDK.Init("54C89CF166E84DED914954E04B729744",channelId,"");
    }
}