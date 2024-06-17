using UnityEngine;

public class MasterHiringTicketBtn : MonoBehaviour
{
    public int TicketID;

    public void ShowTooltip()
    {
        TooltipStorageItem tooltipStorageItem = WidgetManager.Instance.Show<TooltipStorageItem>(WidgetID.TooltipStorageItem);
        tooltipStorageItem.UpdateViewHideCount(this.TicketID,this.transform,StorageType.Ticket,false);
    }
    public void HideTooltip()
    {
        WidgetManager.Instance.Hide(WidgetID.TooltipStorageItem);
        WidgetManager.Instance.Hide(WidgetID.TooltipMessage);
        WidgetManager.Instance.Hide(WidgetID.TooltipPropAction);
        WidgetManager.Instance.Hide(WidgetID.EquipmentInfoWidget);
    }
}