using System;
using System.IO;

using DefaultNamespace.Common;

using JetBrains.Annotations;

using UnityEngine;

[Serializable]
public class HostSaveData
{
    public long AvatarId;
    [SerializeField]
    private bool _hasNewCollection;
    public bool HasNewCollection
    {
        get => this._hasNewCollection;
        set
        {
            this._hasNewCollection = value;
            OnHasNewCollectionChanged?.Invoke(this._hasNewCollection);
            // 性能消耗较小, 经常保存即可. 
            HostSaveDataManager.Instance.SaveHostSaveData();
        }
    }
    public static event Action<bool> OnHasNewCollectionChanged;

    public HostSaveData(long avatarId)
    {
        this.AvatarId = avatarId;
    }
}

public class HostSaveDataManager : SingletonMonoBehaviour<HostSaveDataManager>
{
    private HostSaveData _hostSaveData;
    public HostSaveData HostSaveData
    {
        get
        {
            long myAvatarID = DataManager.Instance.MyAvatarID;
            if (this._hostSaveData == null)
            {
                HostSaveData loadHostSaveData = this.LoadHostSaveData(myAvatarID) ?? new HostSaveData(myAvatarID);
                this._hostSaveData = loadHostSaveData;
            }
            else
            {
                if (this._hostSaveData.AvatarId != myAvatarID)
                {
                    HostSaveData loadHostSaveData = this.LoadHostSaveData(myAvatarID) ?? new HostSaveData(myAvatarID);
                    this._hostSaveData = loadHostSaveData;
                }
            }
            return this._hostSaveData;
        }
    }

    public void SaveHostSaveData()
    {
        string json = JsonUtility.ToJson(this.HostSaveData);
        File.WriteAllText(Path.Combine(Application.persistentDataPath,this.HostSaveData.AvatarId + ".hostsavedata"),json);
    }
    [CanBeNull]
    private HostSaveData LoadHostSaveData(long myAvatarID)
    {
        if (!File.Exists(Path.Combine(Application.persistentDataPath,myAvatarID + ".hostsavedata"))) return null;
        string json = File.ReadAllText(Path.Combine(Application.persistentDataPath,myAvatarID + ".hostsavedata"));
        return JsonUtility.FromJson<HostSaveData>(json);
    }
}