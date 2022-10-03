using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyZone : MonoBehaviour, IDropHandler
{
    public GameObject hand;
    public GameObject board;
    public ShopManager shopManager;

    public void OnDrop(PointerEventData eventData)
    {
        int droppedCardId = 0;
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        ThisCard t = eventData.pointerDrag.GetComponent<ThisCard>();
        ThisItem i = eventData.pointerDrag.GetComponent<ThisItem>();
        ThisChoice c = eventData.pointerDrag.GetComponent<ThisChoice>();
        if (t != null)
            droppedCardId = t.thisId;
        if (i != null)
            droppedCardId = i.thisId;
        //if (c != null)
        //    droppedCardId = c.thisId;

        if (d != null && t != null)
            BuyCard(droppedCardId, d, t);

        if (d != null && i != null)
            BuyItem(droppedCardId, d, i);

        if (d != null && c != null)
            shopManager.ActivateChoice(c);

        gameObject.SetActive(false);
    }

    void BuyCard(int droppedCardId, Draggable d, ThisCard t)
    {
        if (d.typeOfCard == Draggable.Slot.SHOP && shopManager.gold >= t.cost && board.transform.childCount < 7)
        {
            shopManager.gold = shopManager.gold - t.cost;
            d.typeOfCard = Draggable.Slot.BOARD;
            d.returnParent = board.transform;
            d.transform.SetParent(board.transform);


            if (GlobalControl.Instance.ownedRelics.Contains(2) && t.type == "Defect")   // corruption
            {
                t.attack++;
                t.health++;
            }
            if (GlobalControl.Instance.ownedRelics.Contains(3) && t.type == "Chimera")  // dogfood
            {
                t.attack++;
                t.health++;
            }
            if (GlobalControl.Instance.ownedRelics.Contains(4) && t.type == "Sentinel") // capacitor
            {
                t.attack++;
            }

            shopManager.TriggerOnPlay(t);
            shopManager.StartTripleCheck(droppedCardId);
        }
    }

    void BuyItem(int droppedCardId, Draggable d, ThisItem i)
    {
        if (d.typeOfCard == Draggable.Slot.SHOP && shopManager.gold >= i.cost && board.transform.childCount < 7)
        {
            shopManager.gold = shopManager.gold - i.cost;
            d.typeOfCard = Draggable.Slot.BOARD;
            d.returnParent = board.transform;
            d.transform.SetParent(board.transform);
        }
    }
}
