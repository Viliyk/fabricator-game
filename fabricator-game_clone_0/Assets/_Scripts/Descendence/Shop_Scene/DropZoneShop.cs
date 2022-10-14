using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneShop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ShopManager shopManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();

        if (d != null && d.typeOfCard == Draggable.Slot.SHOP)
            d.placeholderParent = transform;
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
        GlobalControl.Instance.shopHighlight.SetActive(false);

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        ThisConsumable c = eventData.pointerDrag.GetComponent<ThisConsumable>();

        if (c != null)
        {
            if (c.type != "Battle")
            {
                shopManager.ConsumableFunctionality(c.id);
                Destroy(c.gameObject, 0.01f);
            }
            return;
        }

        if (d != null)
        {
            if (d.typeOfCard == Draggable.Slot.BOARD)   // sells the card if it has the board enum
            {
                shopManager.gold++;
                Destroy(d.gameObject, 0.01f);
            }
        }
    }
}
