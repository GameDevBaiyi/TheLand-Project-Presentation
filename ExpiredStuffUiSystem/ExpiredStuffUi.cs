using UnityEngine;

public class ExpiredStuffUi : MonoBehaviour
{
    public UITexture UiTexture;
    public UILabel CountLabel;

    public int ConfigId;
    public int Count;

    public void Refresh(int configId,int count)
    {
        this.ConfigId = configId;
        this.Count = count;

        this.gameObject.SetActive(true);
        SuPro.Blueprint.Stuff stuffConfig = BlueprintStorage.Instance.Tables.Stuff.Records.GetOrNull(configId);
        this.UiTexture.mainTexture = ResourceHelper.GetItemIconTextureByName(stuffConfig.Name);
        this.CountLabel.text = (-count).ToString();
    }

    public void OnPressThisBtn()
    {
        if (WidgetManager.Instance.IsActive(WidgetID.TooltipStorageItem))
        {
            WidgetManager.Instance.Hide(WidgetID.TooltipStorageItem);
        }
        WidgetManager.Instance.Show<TooltipStorageItem>(WidgetID.TooltipStorageItem).UpdateView(this.ConfigId,this.transform,StorageType.Stuff,false);
    }
    public void OnReleaseThisBtn()
    {
        WidgetManager.Instance.Hide(WidgetID.TooltipStorageItem);
    }
}