using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Color = UnityEngine.Color;
using Vector2 = UnityEngine.Vector2;

public class MasterHiringWidget : PopupBaseType1
{
    [SerializeField]
    private MasterLookView _masterLookView;

    [SerializeField]
    private UILabel _ticketRCountText;
    [SerializeField]
    private UILabel _ticketSrCountText;
    [SerializeField]
    private UILabel _ticketUrCountText;
    [SerializeField]
    private UILabel _ticketMrCountText;
    [SerializeField]
    private UILabel _ticketLrCountText;
    [SerializeField]
    private List<string> _ticketTextureNames = new List<string>(5);

    [SerializeField]
    private UILabel _durationDescription;

    public List<string> HireDurationList = new List<string>(5);
    [SerializeField]
    private UIPopupList _popupList;
    [SerializeField]
    private UILabel _durationText;

    [SerializeField]
    private TDoubleClickButton _egDoubleClickConfirmBtn;

    [SerializeField]
    private UILabel _egCount;
    [SerializeField]
    private UIButton _ticketConfirmBtn;
    [SerializeField]
    private UITexture _ticketTexture;
    [SerializeField]
    private UILabel _ticketCount;

    private readonly List<int> _ticketIds = new List<int>
    {
        MasterHiringUtilities.RTicketID,MasterHiringUtilities.SrTicketID,MasterHiringUtilities.UrTicketID,MasterHiringUtilities.MrTicketID,
        MasterHiringUtilities.LrTicketID,
    };
    public static bool IsHiring;
    public static MasterInfo MasterToRenew;

    private int _currentSelectedIndex;

    public void Start()
    {
        this.SetupViewTransform(this.transform.Find("Animating"));
        this._egDoubleClickConfirmBtn.targetOnClick.Clear();
        this._egDoubleClickConfirmBtn.targetOnClick.Add(new EventDelegate(this.OnClickEgConfirmBtn));
    }

    public override void Show()
    {
        base.Show();

        // Appearance
        void ShowAppearence()
        {
            if (IsHiring)
            {
                void AckHandler(SuPro.MasteHireInfoAck masteHireInfoAck,NetMessageContext context)
                {
                    if (masteHireInfoAck.Result != SuPro.MasterBaseResult.Success) return;
                    AvatarAppearenceData avatarAppearenceData = new AvatarAppearenceData();
                    avatarAppearenceData.ClearEquipments();
                    foreach (int mountings in masteHireInfoAck.masterInfo.MountingsList)
                    {
                        avatarAppearenceData.SetEquipment(mountings);
                    }
                    this._masterLookView.appearence = avatarAppearenceData;
                    this._masterLookView.ShowAppearence();
                }
                NetworkManager.Instance.SendToTS(new SuPro.MasteHireInfoReq()).Ack<SuPro.MasteHireInfoAck>(AckHandler);
                return;
            }

            if (MasterToRenew == null)
            {
                Debug.LogError("MasterToRenew is null");
                return;
            }

            this._masterLookView.appearence = MasterToRenew.Appearence;
            this._masterLookView.ShowAppearence();
        }
        ShowAppearence();

        StorageTicketData storageTicketData = DataManager.Instance.MyAvatar.Ticket;
        int rTicketCount = storageTicketData.GetCount(MasterHiringUtilities.RTicketID);
        this._ticketRCountText.text = rTicketCount.ToString();
        int srTicketCount = storageTicketData.GetCount(MasterHiringUtilities.SrTicketID);
        this._ticketSrCountText.text = srTicketCount.ToString();
        int urTicketCount = storageTicketData.GetCount(MasterHiringUtilities.UrTicketID);
        this._ticketUrCountText.text = urTicketCount.ToString();
        int mrTicketCount = storageTicketData.GetCount(MasterHiringUtilities.MrTicketID);
        this._ticketMrCountText.text = mrTicketCount.ToString();
        int lrTicketCount = storageTicketData.GetCount(MasterHiringUtilities.LrTicketID);
        this._ticketLrCountText.text = lrTicketCount.ToString();

        string specialSkillDescriptionKey = IsHiring ? "Master_Rental_03" : "Master_Rental_05";
        this._durationDescription.text = StringUtil.GetUIString(specialSkillDescriptionKey);

        this.HireDurationList.Clear();
        this._popupList.items.Clear();
        SuPro.Blueprint.TicketTable ticketTable = BlueprintStorage.Instance.Tables.Ticket;
        string GetFormatedDurationText(float ticketSecs)
        {
            TimeSpan ticketTimeSpan = TimeSpan.FromSeconds(ticketSecs);
            string text;
            if (ticketTimeSpan < TimeSpan.FromHours(1d))
            {
                text = $"{ticketTimeSpan.Minutes}{StringUtil.GetUIString("Time_Min")}";
            }
            else if (ticketTimeSpan < TimeSpan.FromDays(1d))
            {
                text = $"{ticketTimeSpan.Hours}{StringUtil.GetUIString("Time_Hour")}";
            }
            else
            {
                text = $"{ticketTimeSpan.Days}{StringUtil.GetUIString("Time_Day")}";
                if (ticketTimeSpan.Hours > 0) text += $"{ticketTimeSpan.Hours}{StringUtil.GetUIString("Time_Hour")}";
            }
            return text;
        }

        float rTicketSecs = ticketTable[MasterHiringUtilities.RTicketID].CommonVal;
        this.HireDurationList.Add($"{GetFormatedDurationText(rTicketSecs)}");
        float srTicketSecs = ticketTable[MasterHiringUtilities.SrTicketID].CommonVal;
        this.HireDurationList.Add($"{GetFormatedDurationText(srTicketSecs)}");
        float urTicketSecs = ticketTable[MasterHiringUtilities.UrTicketID].CommonVal;
        this.HireDurationList.Add($"{GetFormatedDurationText(urTicketSecs)}");
        float mrTicketSecs = ticketTable[MasterHiringUtilities.MrTicketID].CommonVal;
        this.HireDurationList.Add($"{GetFormatedDurationText(mrTicketSecs)}");
        float lrTicketSecs = ticketTable[MasterHiringUtilities.LrTicketID].CommonVal;
        this.HireDurationList.Add($"{GetFormatedDurationText(lrTicketSecs)}");
        this._popupList.items.AddRange(this.HireDurationList);
        this._durationText.text = this.HireDurationList.FirstOrDefault();
        this._popupList.value = this.HireDurationList.FirstOrDefault();
    }

    public void OnClickFaqBtn()
    {
        UIManager.Instance.ChangeToGUIView<FAQWidget>(WidgetID.FAQ,initializer: faqWidget => { faqWidget.faqGroupID = 6840; });
    }
    public void OnClickCloseBtn()
    {
        UIManager.Instance.ChangeToBasicState();
    }
    public void OnSelectHireDuration()
    {
        this._currentSelectedIndex = this._popupList.items.IndexOf(this._popupList.value);
        this._durationText.text = this._popupList.value;

        SuPro.Blueprint.Constant constant = BlueprintStorage.Instance.Tables.Constant[SuPro.Blueprint.ConstantType.MasterHireGold];
        if (constant == null)
        {
            Debug.LogError("MasterHireGold constant is null");
            return;
        }
        string[] egCountStrings = constant.Value.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
        if (egCountStrings.Length != 5)
        {
            Debug.LogError($"MasterHireGold constant value is invalid: {constant.Value}");
            return;
        }

        int playerEg = DataManager.Instance.MyAvatar.Money.Crystal;
        int requiredEg = int.Parse(egCountStrings[this._currentSelectedIndex]);
        bool hasEnoughEg = playerEg >= requiredEg;
        this._egCount.text = egCountStrings[this._currentSelectedIndex];
        this._egCount.color = hasEnoughEg ? Color.white : Color.red;

        int playerTicketCount = DataManager.Instance.MyAvatar.Ticket.GetCount(this._ticketIds[this._currentSelectedIndex]);
        bool hasEnoughTicket = playerTicketCount > 0;
        this._ticketConfirmBtn.isEnabled = hasEnoughTicket;
        this._ticketTexture.mainTexture
            = (Texture)ResourceHelper.Load(E_BUNDLE_TYPE.GUITexItem,0,this._ticketTextureNames[this._currentSelectedIndex],typeof(Texture));
        this._ticketCount.color = hasEnoughTicket ? Color.white : Color.red;
    }
    public void OnClickEgConfirmBtn()
    {
        UIManager.Instance.ChangeToBasicState();

        if (IsHiring)
        {
            SuPro.Blueprint.Constant constant = BlueprintStorage.Instance.Tables.Constant[SuPro.Blueprint.ConstantType.MasterHireGold];
            string[] egCountStrings = constant.Value.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
            if (egCountStrings.Length != 5)
            {
                Debug.LogError($"MasterHireGold constant value is invalid: {constant.Value}");
                return;
            }
            int playerEg = DataManager.Instance.MyAvatar.Money.Crystal;
            int requiredEg = int.Parse(egCountStrings[this._currentSelectedIndex]);
            bool hasEnoughEg = playerEg >= requiredEg;

            if (!hasEnoughEg)
            {
                this._egDoubleClickConfirmBtn.enabled = false;
                UIManager.Instance.ShowDiamondShopWidget();
                return;
            }

            SuPro.MasteHireReq masteHireReq = new SuPro.MasteHireReq();
            masteHireReq.isEg = true;
            masteHireReq.configId = this._currentSelectedIndex;
            NetworkManager.Instance.SendToTS(masteHireReq).Ack<SuPro.MasteHireAck>(MasteHireAckHandler);

            return;
        }

        SuPro.Blueprint.TicketTable ticketTable = BlueprintStorage.Instance.Tables.Ticket;
        int renewSecs = ticketTable[this._ticketIds[this._currentSelectedIndex]].CommonVal;
        bool isExceed90Days = (MasterToRenew.Data.expireTimer - NetworkManager.Instance.Now) + TimeSpan.FromSeconds(renewSecs)
                            > TimeSpan.FromDays(90);
        if (isExceed90Days)
        {
            MessageWidget messageWidget = (MessageWidget)WidgetManager.Instance.Show(WidgetID.Message);
            void UpdateViewCallack(MessageWidget widget)
            {
                widget.ShowBackscreen();
                widget.SetMessageViewOnUpdateView(StringUtil.GetUIString("Master_Rental_22"));
                widget.SetButtonTypeOnUpdateView(MessageWidget.ButtonType.YesNo);
            }
            void ResultCallback(MessageWidget.Result result)
            {
                if (result != MessageWidget.Result.Yes) return;
                SuPro.MasteRenewReq masteRenewReq = new SuPro.MasteRenewReq();
                masteRenewReq.masterId = MasterToRenew.Data.MasterId;
                masteRenewReq.isEg = true;
                masteRenewReq.configId = this._currentSelectedIndex;
                NetworkManager.Instance.SendToTS(masteRenewReq).Ack<SuPro.MasteRenewAck>(this.MasteRenewAckHandler);
            }
            messageWidget.SetCallback(UpdateViewCallack,ResultCallback);

            return;
        }

        SuPro.MasteRenewReq masteRenewReq = new SuPro.MasteRenewReq();
        masteRenewReq.masterId = MasterToRenew.Data.MasterId;
        masteRenewReq.isEg = true;
        masteRenewReq.configId = this._currentSelectedIndex;
        NetworkManager.Instance.SendToTS(masteRenewReq).Ack<SuPro.MasteRenewAck>(this.MasteRenewAckHandler);
    }
    public void OnClickTicketConfirmBtn()
    {
        UIManager.Instance.ChangeToBasicState();

        if (IsHiring)
        {
            SuPro.MasteHireReq masteHireReq = new SuPro.MasteHireReq();
            masteHireReq.isEg = false;
            masteHireReq.configId = this._ticketIds.ElementAtOrDefault(this._currentSelectedIndex);
            NetworkManager.Instance.SendToTS(masteHireReq).Ack<SuPro.MasteHireAck>(this.MasteHireAckHandler);

            return;
        }

        SuPro.Blueprint.TicketTable ticketTable = BlueprintStorage.Instance.Tables.Ticket;
        int renewSecs = ticketTable[this._ticketIds[this._currentSelectedIndex]].CommonVal;
        bool isExceed90Days = (MasterToRenew.Data.expireTimer - NetworkManager.Instance.Now) + TimeSpan.FromSeconds(renewSecs)
                            > TimeSpan.FromDays(90);

        if (isExceed90Days)
        {
            MessageWidget messageWidget = (MessageWidget)WidgetManager.Instance.Show(WidgetID.Message);
            void UpdateViewCallack(MessageWidget widget)
            {
                widget.ShowBackscreen();
                widget.SetMessageViewOnUpdateView(StringUtil.GetUIString("Master_Rental_22"));
                widget.SetButtonTypeOnUpdateView(MessageWidget.ButtonType.YesNo);
            }
            void ResultCallback(MessageWidget.Result result)
            {
                if (result != MessageWidget.Result.Yes) return;
                SuPro.MasteRenewReq masteRenewReq = new SuPro.MasteRenewReq();
                masteRenewReq.masterId = MasterToRenew.Data.MasterId;
                masteRenewReq.isEg = false;
                masteRenewReq.configId = this._ticketIds.ElementAtOrDefault(this._currentSelectedIndex);
                NetworkManager.Instance.SendToTS(masteRenewReq).Ack<SuPro.MasteRenewAck>(this.MasteRenewAckHandler);
            }
            messageWidget.SetCallback(UpdateViewCallack,ResultCallback);

            return;
        }

        SuPro.MasteRenewReq masteRenewReq = new SuPro.MasteRenewReq();
        masteRenewReq.masterId = MasterToRenew.Data.MasterId;
        masteRenewReq.isEg = false;
        masteRenewReq.configId = this._ticketIds.ElementAtOrDefault(this._currentSelectedIndex);
        NetworkManager.Instance.SendToTS(masteRenewReq).Ack<SuPro.MasteRenewAck>(this.MasteRenewAckHandler);
    }

    private void MasteHireAckHandler(SuPro.MasteHireAck masteHireAck,NetMessageContext context)
    {
        if (masteHireAck.Result != SuPro.MasterBaseResult.Success) return;
        AssetArgs assetArgs = new AssetArgs().Set(masteHireAck.MoneyChangeList).Set(masteHireAck.TicketChangeList);
        JobHelper.PushIf(!assetArgs.IsEmpty(),Jobs.AcquireAssets(Vector2.zero,assetArgs));
        JobHelper.PushIf(masteHireAck.MoneyAchieveEventChangeList,Jobs.ApplyMoneyAchieveEventChanges(masteHireAck.MoneyAchieveEventChangeList));
        DataManager.Instance.MyAvatar.MasterData.UpdateList(masteHireAck.AvatarMasterList);
        WidgetManager.Instance.Alert(AlertMessageType.Info,StringUtil.GetUIString("Master_Rental_10"));
        UIManager.Instance.ChangeToGUIView(WidgetID.MasterListWidget);

        MasterHireDurationTimer.Instance.CheckExpiredMasterAndSendRequest();
    }

    private void MasteRenewAckHandler(SuPro.MasteRenewAck masteRenewAck,NetMessageContext context)
    {
        if (masteRenewAck.Result != SuPro.MasterBaseResult.Success) return;
        AssetArgs assetArgs = new AssetArgs().Set(masteRenewAck.MoneyChangeList).Set(masteRenewAck.TicketChangeList);
        JobHelper.PushIf(!assetArgs.IsEmpty(),Jobs.AcquireAssets(Vector2.zero,assetArgs));
        JobHelper.PushIf(masteRenewAck.MoneyAchieveEventChangeList,Jobs.ApplyMoneyAchieveEventChanges(masteRenewAck.MoneyAchieveEventChangeList));
        DataManager.Instance.MyAvatar.MasterData.UpdateMaster(masteRenewAck.masterInfo);
        WidgetManager.Instance.Alert(AlertMessageType.Info,StringUtil.GetUIString("Master_Rental_10"));
        UIManager.Instance.ChangeToGUIView(WidgetID.MasterListWidget);

        MasterHireDurationTimer.Instance.CheckExpiredMasterAndSendRequest();
    }
}