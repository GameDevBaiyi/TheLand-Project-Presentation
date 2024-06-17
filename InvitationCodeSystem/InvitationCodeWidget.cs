using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using SuPro;
using SuPro.Blueprint;

using UnityEngine;

namespace ScriptsSortedByFunctions.InvitationCodeSystem
{
public class InvitationCodeWidget : PopupBaseType1
{
    private const int _awardConfigId = 10001;
    public const int LevelLimit = 3;

    private bool _hasInitialized;

    [SerializeField]
    private UIInput _uiInput;

    [SerializeField]
    private GameObject _submitOnBtnGo;
    [SerializeField]
    private GameObject _submitOffBtnGo;

    [SerializeField]
    private UIReuseGrid _uiReuseGrid;
    private readonly List<AwardItem> _awardItemsCache = new List<AwardItem>(10);

    private void CheckAndInitialize()
    {
        if (this._hasInitialized) return;

        InvitationReward invitationRewardConfig = BlueprintStorage.Instance.Tables.InvitationReward.Records[_awardConfigId];
        if (invitationRewardConfig.Coin > 0) this._awardItemsCache.Add(new AwardItem(RewardCategory.Coin,invitationRewardConfig.Coin));
        if (invitationRewardConfig.Crystal > 0) this._awardItemsCache.Add(new AwardItem(RewardCategory.Crystal,invitationRewardConfig.Crystal));
        if (invitationRewardConfig.Heart > 0) this._awardItemsCache.Add(new AwardItem(RewardCategory.Heart,invitationRewardConfig.Heart));
        if (invitationRewardConfig.Stuff != null)
        {
            foreach (RewardItem item in invitationRewardConfig.Stuff)
            {
                this._awardItemsCache.Add(new AwardItem(RewardCategory.Stuff,item.Count,item.ItemRef));
            }
        }
        if (invitationRewardConfig.Equipment != null)
        {
            foreach (RewardItem item in invitationRewardConfig.Equipment)
            {
                this._awardItemsCache.Add(new AwardItem(RewardCategory.Equipment,item.Count,item.ItemRef));
            }
        }
        if (invitationRewardConfig.Furniture != null)
        {
            foreach (RewardItem item in invitationRewardConfig.Furniture)
            {
                this._awardItemsCache.Add(new AwardItem(RewardCategory.Furniture,item.Count,item.ItemRef));
            }
        }
        if (invitationRewardConfig.Artifact != null)
        {
            foreach (RewardItem item in invitationRewardConfig.Artifact)
            {
                this._awardItemsCache.Add(new AwardItem(RewardCategory.Artifact,item.Count,item.ItemRef));
            }
        }
        if (invitationRewardConfig.Ticket != null)
        {
            foreach (RewardItem item in invitationRewardConfig.Ticket)
            {
                this._awardItemsCache.Add(new AwardItem(RewardCategory.Ticket,item.Count,item.ItemRef));
            }
        }
        if (invitationRewardConfig.Specialty != null)
        {
            foreach (RewardItem item in invitationRewardConfig.Specialty)
            {
                this._awardItemsCache.Add(new AwardItem(RewardCategory.Specialty,item.Count,item.ItemRef));
            }
        }

        this._hasInitialized = true;
    }

    private void Start()
    {
        this.CheckAndInitialize();
        this.SetupViewTransform(this.transform);
        this.gameObject.SetActive(true);

        this._uiInput.value = null;
        this.StandardizeInput();

        this._uiReuseGrid.ResetPosition();
        this._uiReuseGrid.ClearItem(false);
        this._uiReuseGrid.ResetPosition();
        this._uiReuseGrid.ClearItem(false);
        foreach (AwardItem awardItem in this._awardItemsCache)
        {
            this._uiReuseGrid.AddItem(new AwardCellData(awardItem,false),false);
        }
        this._uiReuseGrid.transform.parent.GetComponent<UIReuseScrollView>().DisableSpring();
        this._uiReuseGrid.UpdateAll();
    }

    public void Close()
    {
        UIManager.Instance.ChangeToBasicState();
    }

    public void StandardizeInput()
    {
        this._uiInput.value = Regex.Replace(this._uiInput.value,@"\W","");
        bool hasContent = !string.IsNullOrEmpty(this._uiInput.value);
        this._submitOnBtnGo.SetActive(hasContent);
        this._submitOffBtnGo.SetActive(!hasContent);
    }

    public void SubmitInvitationCode()
    {
        InvitationCodeLogic.SendInvitationBindReq(this._uiInput.value,_awardConfigId);
    }

    public static bool CanInvite()
    {
        bool isLowerThanLvLimit = DataManager.Instance.MyAvatar.Level < LevelLimit;
        if (isLowerThanLvLimit) return false;
        bool hasInvited = !DataManager.Instance.MyAvatar.CanBandInvitationCode;
        if (hasInvited) return false;
        bool hasExpired = (NetworkManager.Instance.Now - InvitationCodeLogic.ActivityStartTime) > TimeSpan.FromHours(InvitationCodeButton.DurationInHours);
        if (hasExpired) return false;

        return true;
    }
}
}