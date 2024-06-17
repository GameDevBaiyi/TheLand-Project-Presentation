#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;

using SuPro;
using SuPro.Blueprint;

using UnityEngine;

namespace DefaultNamespace
{
public class TestSomething : MonoBehaviour
{
    public int EquipmentConfigId;
    public CharacterBase CharacterBase;
    public void ChangeEquipment()
    {
        Equipment equipmentConfig = BlueprintStorage.Instance.Tables.Equipment[this.EquipmentConfigId];
        this.CharacterBase.ChangeEquipment(equipmentConfig,true,false);
    }

    public float TimeScale = 1f;
    public void SetTimeScale()
    {
        Time.timeScale = this.TimeScale;
    }

    public string AnimeName;
    public void PlayAnime()
    {
        // this.CharacterBase.ChangeState(new CharacterStatePlayAnim(this.AnimeName,true));
        IronSource.Agent.setConsent(true);
    }

    public void Test()
    {
        KeyValuePair<int,Ticket> keyValuePair = BlueprintStorage.Instance.Tables.Ticket.Records.FirstOrDefault();
        KeyValuePair<int,StorageTicketItemData> firstOrDefault = DataManager.Instance.MyAvatar.Ticket.Items.FirstOrDefault();
        if (firstOrDefault.Value == null) return;
    }

    private Master Master;
    private const float _requestInterval = 3f;
    private float _timer;
    private void Update()
    {
        this.Master ??= this.GetComponent<Master>();
        if (this.Master == null) return;
        this._timer += Time.deltaTime;
        if (this._timer < _requestInterval) return;
        this._timer = 0f;
        GridObjectController gridObjectController = MasterData.FindCtrl(this.Master.Data.WorkInfo.WorkKind,this.Master.Data.WorkInfo.LastPosId);
        MasterManager.Instance.WorkActionChange(new MasterWorkActionChangeArgs(this.Master.Data.MasterID,MasterWorkActionChangeArgs.ActionType.MoveTo,
                                                                               this.Master.Data.WorkInfo.WorkKind,gridObjectController.ID.No));
    }
}
}
#endif