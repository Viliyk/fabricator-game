using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneTrader : MonoBehaviour, IDropHandler
{
    public TraderManager traderManager;

    public void OnDrop(PointerEventData eventData)
    {
        GlobalControl.Instance.shopHighlight.SetActive(false);

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        ThisCard t = eventData.pointerDrag.GetComponent<ThisCard>();

        if (d != null && t != null)
        {
            if (d.typeOfCard == Draggable.Slot.BOARD)   // sells the card if it has the board enum
                traderManager.SellMinion(t);
        }
    }
}