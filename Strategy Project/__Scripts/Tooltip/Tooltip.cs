using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerMoveHandler
{
    public string message;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Tooltip>() != null && GetComponent<Button>().onClick.GetPersistentEventCount() != 0)
        {
            TooltipManager._instance.SetAndShowTooltip(message);

            

            TooltipManager._instance.SetTooltipScale();

        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager._instance.HideTooltip();

    }

    public void OnPointerMove(PointerEventData eventData)
    {
        TooltipManager._instance.SetTooltipScale();
    }
}
