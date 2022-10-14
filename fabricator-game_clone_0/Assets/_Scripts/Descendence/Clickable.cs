using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject cardTemplate = null;

    ThisCard thisCard;

    private void Awake()
    {
        thisCard = cardTemplate.GetComponent<ThisCard>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        thisCard.TargetThis();
        gameObject.SetActive(false);
    }
}
