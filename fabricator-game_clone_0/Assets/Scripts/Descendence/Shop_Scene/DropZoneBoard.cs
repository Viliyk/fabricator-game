using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneBoard : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ShopManager shopManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
            d.placeholderParent = this.transform;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null && d.placeholderParent == this.transform)
            d.placeholderParent = d.returnParent;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GlobalControl.Instance.boardHighlight.SetActive(false);

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        ThisCard t = eventData.pointerDrag.GetComponent<ThisCard>();
        ThisConsumable c = eventData.pointerDrag.GetComponent<ThisConsumable>();

        //if (c != null)
        //{
        //    if (c.type != "Battle")
        //    {
        //        shopManager.ConsumableFunctionality(c.id);
        //        Destroy(c.gameObject, 0.01f);
        //    }
        //    return;
        //}

        if (d != null && t != null)
        {
            if (d.typeOfCard == Draggable.Slot.HAND && transform.childCount <= 7)
            {
                d.typeOfCard = Draggable.Slot.BOARD;    // change the card to board enum
                d.returnParent = transform;
                d.transform.SetParent(transform);

                shopManager.TriggerOnPlay(t);       // trigger events related to playing a minion
            }
        }
    }
}
