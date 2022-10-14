using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backline : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public BattleManager battleManager;

    Draggable draggable;
    ThisCard thisCard;
    bool mouseOver = false;

    void Update()
    {
        // set placeholder if hovered card is playable
        if (mouseOver = true && draggable != null && thisCard != null)
        {
            if (draggable.typeOfCard == Draggable.Slot.BATTLEHAND && transform.childCount < battleManager.backlineSlots && thisCard.energyCost <= battleManager.energy)
            {
                draggable.placeholderParent = transform;
                draggable.SetPlaceholder();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        mouseOver = true;

        draggable = eventData.pointerDrag.GetComponent<Draggable>();
        thisCard = eventData.pointerDrag.GetComponent<ThisCard>();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;

        if (eventData.pointerDrag == null)
            return;

        draggable = eventData.pointerDrag.GetComponent<Draggable>();

        if (draggable != null && draggable.placeholderParent == transform)
            draggable.placeholderParent = draggable.returnParent;
    }

    public void OnDrop(PointerEventData eventData)
    {
        mouseOver = false;

        if (GlobalControl.Instance.targetMode)
            return;
        //GlobalControl.Instance.boardHighlight.SetActive(false);

        GameObject droppedCard = eventData.pointerDrag.gameObject;
        Draggable d = droppedCard.GetComponent<Draggable>();
        ThisCard t = droppedCard.GetComponent<ThisCard>();
        ThisItem i = droppedCard.GetComponent<ThisItem>();
        ThisConsumable c = droppedCard.GetComponent<ThisConsumable>();

        //GameObject spawnedCard;

        // use item
        if (d != null && i != null)
        {
            if (d.typeOfCard == Draggable.Slot.BATTLE)
                battleManager.ItemFunctionality(i, null);
            return;
        }

        if (d != null && t != null)
        {
            if (d.typeOfCard == Draggable.Slot.BATTLEHAND)
            {
                droppedCard.transform.localScale = new Vector3(0.73f, 0.73f, 1);

                //d.returnParent = transform;
                //d.transform.SetParent(transform);
                //d.OnEndDrag(eventData);
                //d.enabled = false;

                //battleManager.SpawnToBackline(droppedCard, false);

                //spawnedCard = Instantiate(droppedCard);
                //spawnedCard.transform.SetParent(transform);
                //battleManager.TriggerOnPlay(spawnedCard.GetComponent<ThisCard>());

                //battleManager.TriggerOnPlay(t);
                //battleManager.PayMinerals(t.tier * 100);
            }
        }
    }
}