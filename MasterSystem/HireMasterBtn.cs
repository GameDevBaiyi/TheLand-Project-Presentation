using UnityEngine;

public class HireMasterBtn : MonoBehaviour
{
    public void OnClick()
    {
        MasterHiringWidget.IsHiring = true;
        UIManager.Instance.ChangeToGUIView<MasterHiringWidget>(WidgetID.MasterHiringWidget);
    }
}