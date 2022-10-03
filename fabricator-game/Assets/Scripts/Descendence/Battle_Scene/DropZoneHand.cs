using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneHand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    void Update()
    {
        //foreach (Transform card in transform)
        //    card.localScale = new Vector3(0.7f, 0.7f, 1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        RectTransform rt = GetComponent<RectTransform>();
        //transform.localScale = new Vector3(1.8f, 1.8f, 1);
        //transform.localPosition = new Vector3(0, -174.82f, 0);
        rt.anchoredPosition = new Vector2(0, -170);

        if (eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null && d.typeOfCard == Draggable.Slot.HAND)
            d.placeholderParent = transform;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RectTransform rt = GetComponent<RectTransform>();
        //transform.localScale = new Vector3(1, 1, 1);
        //transform.localPosition = new Vector3(0, -214, 0);
        rt.anchoredPosition = new Vector2(0, -215);

        if (eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null && d.placeholderParent == transform)
            d.placeholderParent = d.returnParent;
    }
}
