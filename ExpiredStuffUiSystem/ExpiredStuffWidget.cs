using System.Collections.Generic;

using SuPro;

using UnityEngine;

public class ExpiredStuffWidget : PopupBaseType1
{
    public UIGrid UiGrid;
    public GameObject ItemPrefab;

    public void Start()
    {
        this.SetupViewTransform(this.transform.Find("View"));
    }

    public override void Show()
    {
        base.Show();

        this.ItemPrefab.SetActive(false);
        this.UiGrid.transform.DestroyChildren();
        List<BarnItem> expireBarn = DataManager.Instance.MyAvatar.ExpireBarn;
        for (int i = 0; i < expireBarn.Count; i++)
        {
            GameObject itemUiGo = this.UiGrid.gameObject.AddChild(this.ItemPrefab);
            BarnItem barnItem = expireBarn[i];
            itemUiGo.GetComponent<ExpiredStuffUi>().Refresh(barnItem.BarnItemRef,(int)barnItem.BarnItemCount);
        }

        this.UiGrid.Reposition();
    }

    public void OnClickCloseBtn()
    {
        if (this.IsExpanded() == false) return;
        DataManager.Instance.MyAvatar.ExpireBarn = null;
        UIManager.Instance.ChangeToBasicState();
    }
}