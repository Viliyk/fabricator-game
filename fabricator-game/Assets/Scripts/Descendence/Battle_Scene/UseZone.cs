using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseZone : MonoBehaviour, IDropHandler
{
    public BattleManager battleManager;

    public void OnDrop(PointerEventData eventData)
    {
        ThisConsumable c = eventData.pointerDrag.GetComponent<ThisConsumable>();

        if (c != null)
        {
            battleManager.ConsumableFunctionality(c, null);
            GlobalControl.Instance.battleConsumables.Remove(c.id);
            Destroy(c.gameObject, 0.01f);
        }
    }
}
