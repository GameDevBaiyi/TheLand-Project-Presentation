using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using SuPro;

using UnityEngine;

public static class TalkingDataUtilities
{
    public static void Login(long avatarId)
    {
        TalkingDataProfile profile = TalkingDataProfile.CreateProfile();
        if (avatarId == 0L)
        {
            Debug.LogError("MyAvatarID is 0. ");
            return;
        }
        DCon.Log($"TalkingDataSDK OnLogin, AvatarID: {avatarId}");
        TalkingDataSDK.OnLogin($"{avatarId}",profile);
    }

    public static void RecordPayment(string orderId,double amount,string currencyType)
    {
#if TD_RETAIL
        TalkingDataOrder talkingDataOrder = TalkingDataOrder.CreateOrder(orderId,(int)(amount),currencyType);
        TalkingDataSDK.OnOrderPaySucc(talkingDataOrder,"",GameManager.Account.UserID.ToString(CultureInfo.InvariantCulture));
#endif
    }

    public const int StarSeedId = 50100;
    public const int GoldCrop = 50203;
    public const int SilverCrop = 50201;
    public const int CopperCrop = 50200;
    public static void RecordGetSpecialCrop(int cropId,List<SiloChangeBase> siloChangeList)
    {
        if (siloChangeList == null) return;
        int count = (int)siloChangeList.Where(t => t.SiloItemChangeCount.SiloItemRef == cropId).Sum(t => t.SiloItemChangeCount.DeltaItemCount);
        if (count <= 0) return;
        Dictionary<string,object> parameters = new Dictionary<string,object>();
        parameters["Count"] = count;
        string eventId = null;
        switch (cropId)
        {
        case StarSeedId:
            eventId = DataManager.Instance.Town.IsNftLand() ? "GetStarSeed_NftLand" : "GetStarSeed_MainLand";
            break;

        case GoldCrop:
            eventId = DataManager.Instance.Town.IsNftLand() ? "GetGoldCrop_NftLand" : "GetGoldCrop_MainLand";
            break;

        case SilverCrop:
            eventId = DataManager.Instance.Town.IsNftLand() ? "GetSilverCrop_NftLand" : "GetSilverCrop_MainLand";
            break;

        case CopperCrop:
            eventId = DataManager.Instance.Town.IsNftLand() ? "GetCopperCrop_NftLand" : "GetCopperCrop_MainLand";
            break;
        }

        TalkingDataSDK.OnEvent(eventId,parameters);
    }
    public static void RecordAllGetSpecialCrops(List<SiloChangeBase> siloChangeList)
    {
        RecordGetSpecialCrop(StarSeedId,siloChangeList);
        RecordGetSpecialCrop(GoldCrop,siloChangeList);
        RecordGetSpecialCrop(SilverCrop,siloChangeList);
        RecordGetSpecialCrop(CopperCrop,siloChangeList);
    }

    public static void RecordConsumeStarSeed(int count)
    {
        if (count <= 0) return;
        Dictionary<string,object> parameters = new Dictionary<string,object>();
        parameters["Count"] = count;

        string eventId = DataManager.Instance.Town.IsNftLand() ? "ConsumeStarSeed_NftLand" : "ConsumeStarSeed_MainLand";
        TalkingDataSDK.OnEvent(eventId,parameters);
    }

    public static void RecordConsumeLuckyPower()
    {
        Dictionary<string,object> parameters = new Dictionary<string,object>();
        parameters["Count"] = 1;
        string eventId = "ConsumeLuckyPower";
        TalkingDataSDK.OnEvent(eventId,parameters);
    }

    public static void RecordAdWatching()
    {
        Dictionary<string,object> parameters = new Dictionary<string,object>();
        parameters["Count"] = 1;
        string eventId = "WatchAdAndGetRewarded";
        TalkingDataSDK.OnEvent(eventId,parameters);
    }

    private static void RecordEvent(string eventId,bool isUnique = false)
    {
        if (isUnique)
        {
            bool hasRecorded = PlayerPrefs.HasKey(eventId);
            if (hasRecorded) return;
            PlayerPrefs.SetInt(eventId,1);
        }

        Dictionary<string,object> parameters = new Dictionary<string,object>();
        parameters["Count"] = 1;
        TalkingDataSDK.OnEvent(eventId,parameters);
    }
    public static List<string> CacheRecordedUniqueEvents()
    {
        List<string> recordedEvents = new List<string>(20);

        if (PlayerPrefs.HasKey("GateSuccess")) recordedEvents.Add("GateSuccess");
        if (PlayerPrefs.HasKey("BundleMetaSuccess")) recordedEvents.Add("BundleMetaSuccess");
        if (PlayerPrefs.HasKey("LoginWalletStart")) recordedEvents.Add("LoginWalletStart");
        if (PlayerPrefs.HasKey("LoginWalletSuccess")) recordedEvents.Add("LoginWalletSuccess");
        if (PlayerPrefs.HasKey("AvatarStart")) recordedEvents.Add("AvatarStart");
        if (PlayerPrefs.HasKey("AvatarSuccess")) recordedEvents.Add("AvatarSuccess");
        if (PlayerPrefs.HasKey("StartTutorial")) recordedEvents.Add("StartTutorial");
        if (PlayerPrefs.HasKey("RegisterWallet")) recordedEvents.Add("RegisterWallet");
        if (PlayerPrefs.HasKey("CompletePasscode")) recordedEvents.Add("CompletePasscode");

        return recordedEvents;
    }
    public static void RecoverRecordedUniqueEvents(List<string> recordedEvents)
    {
        foreach (string eventId in recordedEvents)
        {
            PlayerPrefs.SetInt(eventId,1);
        }
    }

    public static void RecordReachLv5()
    {
        RecordEvent("ReachLv5");
    }

    public static void GateSuccess()
    {
        RecordEvent("GateSuccess",true);
    }

    public static void BundleMetaSuccess()
    {
        RecordEvent("BundleMetaSuccess",true);
    }

    public static void LoginWalletStart()
    {
        RecordEvent("LoginWalletStart",true);
    }

    public static void LoginWalletSuccess()
    {
        RecordEvent("LoginWalletSuccess",true);
    }

    public static void AvatarStart()
    {
        RecordEvent("AvatarStart",true);
    }

    public static void AvatarSuccess()
    {
        RecordEvent("AvatarSuccess",true);
    }

    public static void StartTutorial()
    {
        RecordEvent("StartTutorial",true);
    }

    public static void RegisterWallet()
    {
        RecordEvent("RegisterWallet",true);
    }

    public static void CompletePasscode()
    {
        RecordEvent("CompletePasscode",true);
    }
}