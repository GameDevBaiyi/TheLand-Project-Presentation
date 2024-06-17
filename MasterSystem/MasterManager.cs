using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UniRx;

public class MasterManager : ManagerBase<MasterManager>
{
    public KVList<MasterInfo,Master> Masters = new KVList<MasterInfo,Master>();

    private Dictionary<InstanceID,MasterWorkActionChangeArgs> delayedWorkActionChange = new Dictionary<InstanceID,MasterWorkActionChangeArgs>();

    public MasterManager()
    {
        WorldEventManager.Instance.OnSceneBeforeChangingAsObservable().Subscribe(_ => OnSceneBeforeChanging(_));
        WorldEventManager.Instance.OnScenePrepareAsObservable().Subscribe(_ => OnScenePreparing(_));
    }
    
    public override void UnloadTown(bool switchTown)
    {
        var mastersToRemove = this.Masters.ToArray();

        foreach (var pair in mastersToRemove)
        {
            Despawn(pair.Key);
        }
    }

    public void Despawn(MasterInfo data)
    {
        var master = this.Masters.GetOrNull(data);
        if (master != null)
        {
            master.OnFreeResource();
            
            //GameObject.Destroy(master.gameObject);
            ResourceHelper.FreePool(E_POOL_TYPE.CharacterPart, "master", master.gameObject);
            
        }
        this.Masters.Remove(data);
    }
    public void Despawn(long masterId)
    {
        KeyValuePair<MasterInfo,Master> firstOrDefault = default;
        for (int i = 0; i < this.Masters.Count; i++)
        {
            if (this.Masters[i].Key.MasterID == masterId)
            {
                firstOrDefault = this.Masters[i];
            }
        }
        var master = firstOrDefault.Value;
        if (master != null)
        {
            master.OnFreeResource();
            
            //GameObject.Destroy(master.gameObject);
            ResourceHelper.FreePool(E_POOL_TYPE.CharacterPart, "master", master.gameObject);
            
        }
        this.Masters.Remove(firstOrDefault.Key);
    }
    
    public void Spawn(MasterInfo masterInfo)
    {
        if (masterInfo == null) return;

        ApplySpawnLoc(masterInfo);
    }
    
    void OnScenePreparing(EventBase msg)
    {
        SceneChangedEventArgs args = (SceneChangedEventArgs)msg.GetParam(0);

        if (args.sceneType != SceneType.Town) return;

        foreach (var pair in this.Masters)
        {
            ApplySpawnLoc(pair.Key);
        }
    }

    void OnSceneBeforeChanging(EventBase msg)
    {
        foreach (var pair in this.Masters)
        {
            if (pair.Value != null)
            {
                pair.Value.ChangeState(new CharacterStateIdle(), false);
            }
        }
    }
    
    void ApplySpawnLoc(MasterInfo masterInfo)
    {
        var isActive = masterInfo.SpawnLoc.indoor != DataManager.Instance.IsOutdoor();
        if (this.Masters.TryGetValue(masterInfo, out Master master))
        {
            if (master == null)
            {
                if (isActive)
                {
                    this.Masters.Set(masterInfo, CreateMaster(masterInfo));
                }
            }
            else
            {
                master.gameObject.SetActive(isActive);
            }
        }
        else
        {
            if (isActive)
            {
                this.Masters.Add(masterInfo, CreateMaster(masterInfo));
            }
            else
            {
                this.Masters.Add(masterInfo, null);
            }
        }
    }

    public Master CreateMaster(MasterInfo masterInfo)
    {
        GameObject master = ResourceHelper.LoadPool(E_POOL_TYPE.CharacterPart, "master");

        if (master == null)
        {
            master = new GameObject("master character");
        }

        Master character = master.GetOrAddComponent<Master>();

        InstanceID instanceID = new InstanceID(IdentifierType.Master, masterInfo.MasterID);

        master.GetOrAddComponent<Identity>().Set(instanceID, 0);

        // Insert Talk component
        master.GetOrAddComponent<Talk>();

        master.GetOrAddComponent<CharacterWidgetHolder>();
        master.GetOrAddComponent<CharacterShadow>().InitShadow();
        master.GetOrAddComponent<Seeker>();
        master.GetOrAddComponent<Pathfinding.FunnelModifier>();
        master.GetOrAddComponent<TraitCoroutineQ>();
        master.GetOrAddComponent<MasterAi>();

        character.SetData(masterInfo);
        character.isOutdoor = DataManager.Instance.GetWorldScene().InteriorID == InstanceID.InvalidID;
        character.moveType = CharacterBase.MoveType.Undefined;
        character.UpdatePositionFromSpawnLoc();
        character.SetPosition(new Vector2(masterInfo.Position.x, masterInfo.Position.y));
        character.SetHeight(masterInfo.Position.height);
        character.SetAngle(masterInfo.Position.angle);
        character.ForceUpdatePhysics();

        character.ChangeState(new CharacterStateIdle(), true);
        character.ApplyOutfit();

        // attach to character group
        master.transform.parent = GameManager.Scene.CharacterGroup.transform;
        master.transform.localPosition = Vector3.zero;
        
        //apply last workActionChange
        if (delayedWorkActionChange.TryGetValue(instanceID, out var args))
        {
            character.OnWorkActionChange(args);
            delayedWorkActionChange.Remove(instanceID);
        }

        master.GetComponent<MasterAi>().StartRandomWalk();
        return character;
    }
    
    public void ApplyQualitySetting()
    {
        foreach (var master in this.Masters)
        {
            if (master.Value != null)
            {
                master.Value.GetComponent<CharacterShadow>().InitShadow();
            }
        }
    }

    public Master FindMaster(InstanceID instanceID)
    {
        return this.Masters.FirstOrDefault(_ => _.Key.MasterID == instanceID.No).Value;
    }
    
    public Master FindMaster(long masterId)
    {
        return this.Masters.FirstOrDefault(_ => _.Key.MasterID == masterId).Value;
    }

    public void WorkActionChange(MasterWorkActionChangeArgs args)
    {
        var master = FindMaster(args.instanceID);
        if (master != null)
        {
            master.OnWorkActionChange(args);
        }
        else
        {
            delayedWorkActionChange[args.instanceID] = args;
        }
    }

    public bool IsActive(long masterId)
    {
        var master = this.Masters.FirstOrDefault(_ => _.Key.MasterID == masterId).Value;
        if (master != null)
        {
            return master.isOutdoor == DataManager.Instance.IsOutdoor();
        }

        return false;
    }

    public void UpdateAllMastersWorkPos()
    {
        // Update all masters' work positions.
        foreach (KeyValuePair<MasterInfo,Master> keyValuePair in this.Masters)
        {
            Master master = keyValuePair.Value;
            GridObjectController gridObjectController = MasterData.FindCtrl(master.Data.WorkInfo.WorkKind,master.Data.WorkInfo.LastPosId);
            MasterManager.Instance.WorkActionChange(new MasterWorkActionChangeArgs(master.Data.MasterID,MasterWorkActionChangeArgs.ActionType.MoveTo,
                                                                                   master.Data.WorkInfo.WorkKind,gridObjectController.ID.No));
        }
    }
}
