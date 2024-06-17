using UnityEngine;

// 图鉴按钮. 管理红点. 
public class CollectionBtn : MonoBehaviour
{
    public GameObject RedPointGo;

    private void Start()
    {
        HostSaveData.OnHasNewCollectionChanged += this.OnHostSaveDataOnOnHasNewCollectionChanged;
    }
    private void OnDestroy()
    {
        HostSaveData.OnHasNewCollectionChanged -= this.OnHostSaveDataOnOnHasNewCollectionChanged;
    }

    public void RefreshRedPoint()
    {
        this.RedPointGo.SetActive(HostSaveDataManager.Instance.HostSaveData.HasNewCollection);
    }

    private void OnHostSaveDataOnOnHasNewCollectionChanged(bool hasNewCollection)
    {
        this.RedPointGo.SetActive(hasNewCollection);
    }
}