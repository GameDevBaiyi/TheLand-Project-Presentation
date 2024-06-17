using System;
using SuPro;

using Vector2 = UnityEngine.Vector2;

namespace ScriptsSortedByFunctions.InvitationCodeSystem
{
public static class InvitationCodeLogic
{
    // 活动开始的起始时间, 目前是到达三级的时间.
    public static DateTime ActivityStartTime { get; set; }

    /// <summary>
    /// 玩家到达指定等级时, 会记录该起始时间. 并且发送服务器请求记录该时间. 
    /// </summary>
    public static void CheckLevelAndRecordStartTime(int currentLevel)
    {
        if (currentLevel != InvitationCodeWidget.LevelLimit) return;
        NetworkManager.Instance.SendToTS(new AvatarCompleteTutorialReq());
        string ResultGetter(AvatarQueryTimeAck avatarQueryTimeAck) => avatarQueryTimeAck.Result.ToString();
        void AckHandler(AvatarQueryTimeAck avatarQueryTimeAck,NetMessageContext netMessageContext) => ActivityStartTime = avatarQueryTimeAck.Date;
        NetworkManager.Instance.SendToTS(new AvatarQueryTimeReq()).Ack<AvatarQueryTimeAck>(ResultGetter,AckHandler);
    }

    public static void SendInvitationBindReq(string invitationCode,int rewardId)
    {
        InvitationBindReq invitationBindReq = new InvitationBindReq() { InvitationCode = invitationCode,RewardId = rewardId };
        string ResultGetter(InvitationBindAck invitationBindAck) => invitationBindAck.Result.ToString();
        void AckHandler(InvitationBindAck invitationBindAck,NetMessageContext context)
        {
            bool isSuccessful = invitationBindAck.Result == InvitationResult.Success;
            DataManager.Instance.MyAvatar.CanBandInvitationCode = !isSuccessful;
            ShowInvitationTips(isSuccessful);
            if (!isSuccessful) return;
            AssetArgs assetArgs = new AssetArgs().Set(invitationBindAck.MoneyChangeList)
                                                 .Set(invitationBindAck.SiloChangeList)
                                                 .Set(invitationBindAck.BarnChangeList)
                                                 .Set(invitationBindAck.ClothBagChangeList)
                                                 .Set(invitationBindAck.ArtifactChangeList)
                                                 .Set(invitationBindAck.FurnitureChangeList)
                                                 .Set(invitationBindAck.TicketChangeList)
                                                 .Set(invitationBindAck.SpecialtyChangeList);
            JobHelper.PushIf(!assetArgs.IsEmpty(),Jobs.AcquireAssets(Vector2.zero,assetArgs));
            NonAssetArgs nonAssetArgs = NonAssetArgs.New().Set(invitationBindAck.MailChangeList);
            JobHelper.Push(Jobs.ApplyNonAssets(nonAssetArgs));

            MainHUDWidget mainHUDWidget = WidgetManager.Instance.Get<MainHUDWidget>(WidgetID.MainHUD);
            mainHUDWidget.InvitationCodeBtn.CheckAndShowButton();
            InvitationCodeWidget invitationCodeWidget = WidgetManager.Instance.Get<InvitationCodeWidget>(WidgetID.InvitationCodeWidget);
            invitationCodeWidget.Close();
        }

        NetworkManager.Instance.SendToTS(invitationBindReq).Ack<InvitationBindAck>(ResultGetter,AckHandler).NoAutoWarning();
    }

    public static void ShowInvitationTips(bool isSucceed)
    {
        string messageStringId = isSucceed ? "Invitation_Code_06" : "Invitation_Code_05";
        MessageWidget messageWidget = (MessageWidget)WidgetManager.Instance.Show(WidgetID.Message);
        void UpdateViewCallback(MessageWidget widget)
        {
            widget.ShowBackscreen();
            widget.SetTitleOnUpdateView(StringUtil.GetUIString("Notice"));
            widget.SetMessageViewOnUpdateView(StringUtil.GetUIString(messageStringId));
            widget.SetButtonTypeOnUpdateView(MessageWidget.ButtonType.Okay);
        }
        messageWidget.SetCallback(UpdateViewCallback,_ => { });
    }
}
}