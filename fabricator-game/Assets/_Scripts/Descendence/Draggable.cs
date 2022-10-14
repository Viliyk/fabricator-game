using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject highlightBorder = null;

    public enum Slot { PLAYER, SHOP, BOARD, HAND, BATTLE, BATTLEHAND, BUILDING, CHOICE };
    public Slot typeOfCard;

    public Transform returnParent = null;
    public Transform placeholderParent = null;

    // *******placeholder seems to count as a child when checking boards childcount*******
    GameObject placeholder = null;

    public int index;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GlobalControl.Instance.targetMode)
            return;

        placeholder = new GameObject();
        placeholder.transform.localScale = transform.localScale;
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0f;
        le.flexibleHeight = 0f;
        placeholder.transform.SetParent(transform.parent);
        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());

        returnParent = transform.parent;
        placeholderParent = returnParent;
        transform.SetParent(transform.root);

        GetComponent<CanvasGroup>().blocksRaycasts = false;

        ThisCard t = GetComponent<ThisCard>();

        if (typeOfCard == Slot.SHOP || typeOfCard == Slot.CHOICE)    // enable buy zone when you start dragging shop cards
            GlobalControl.Instance.buyZone.SetActive(true);
        //if (typeOfCard == Slot.BATTLE)
        //    GlobalControl.Instance.useZone.SetActive(true);
        if (typeOfCard == Slot.BOARD)
            GlobalControl.Instance.shopHighlight.SetActive(true);
        if (typeOfCard == Slot.HAND)
            GlobalControl.Instance.boardHighlight.SetActive(true);

        //if (t != null)
        //{
        //    //if (typeOfCard == Slot.BATTLEHAND && !t.structure)
        //    //    GlobalControl.Instance.frontlineHighlight.SetActive(true);
        //    if (typeOfCard == Slot.BATTLEHAND)
        //    {
        //        GlobalControl.Instance.backlineHighlight.SetActive(true);
        //    }
        //}
        //if (typeOfCard == Slot.BATTLE)
        //    transform.localScale = new Vector3(0.2f, 0.2f, 1);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GlobalControl.Instance.targetMode)
            return;

        transform.localScale = new Vector3(0.5f, 0.5f, 1);

        transform.position = eventData.position;

        SetPlaceholder();

        highlightBorder.SetActive(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDrag();
    }

    public void EndDrag()
    {
        if (typeOfCard == Slot.SHOP || typeOfCard == Slot.CHOICE)    // disable buy zone when you stop dragging shop cards
            GlobalControl.Instance.buyZone.SetActive(false);
        //if (typeOfCard == Slot.BATTLE)
        //    GlobalControl.Instance.useZone.SetActive(false);
        if (typeOfCard == Slot.BOARD)
            GlobalControl.Instance.shopHighlight.SetActive(false);
        if (typeOfCard == Slot.HAND)
            GlobalControl.Instance.boardHighlight.SetActive(false);

        if (GlobalControl.Instance.frontlineHighlight != null)
            GlobalControl.Instance.frontlineHighlight.SetActive(false);
        if (GlobalControl.Instance.backlineHighlight != null)
            GlobalControl.Instance.backlineHighlight.SetActive(false);

        if (returnParent != null)
            transform.SetParent(returnParent);

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        transform.localScale = new Vector3(0.73f, 0.73f, 1);    // return to regular size

        if (placeholder != null)
        {
            transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            Destroy(placeholder);
        }

        transform.SetSiblingIndex(index);

        DisableHighlight();

        //if (typeOfCard == Slot.BATTLE)
        //    transform.localScale = new Vector3(0.5f, 0.5f, 1);
    }

    public void OnPointerEnter(PointerEventData eventData)      // enable highlight on mouse-over
    {
        if (eventData.pointerDrag == null)
            highlightBorder.SetActive(true);

        //if (typeOfCard == Slot.BATTLEHAND || typeOfCard == Slot.BATTLE || typeOfCard == Slot.BUILDING)
        //{
        //    RectTransform rt = GetComponent<RectTransform>();

        //    rt.anchoredPosition += new Vector2(0, 45);
        //    //transform.position += new Vector3(0, 40, 0);
        //    transform.localScale = new Vector3(0.8f, 0.8f, 1);
        //}
    }

    public void OnPointerExit(PointerEventData eventData)       // disable highlight
    {
        DisableHighlight();

        //if (typeOfCard == Slot.BATTLEHAND || typeOfCard == Slot.BATTLE || typeOfCard == Slot.BUILDING)
        //{
        //    RectTransform rt = GetComponent<RectTransform>();

        //    rt.anchoredPosition -= new Vector2(0, 45);
        //    //transform.position += new Vector3(0, -40, 0);
        //    transform.localScale = new Vector3(0.73f, 0.73f, 1);
        //}
    }

    public void SetPlaceholder()
    {
        if (placeholder == null)
            return;

        if (placeholder.transform.parent != placeholderParent)
            placeholder.transform.SetParent(placeholderParent);

        int newSiblingIndex = placeholderParent.childCount;

        for (int i = 0; i < placeholderParent.childCount; i++)
        {
            if (transform.position.x < placeholderParent.GetChild(i).position.x)
            {
                newSiblingIndex = i;

                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex--;
                break;
            }
        }
        placeholder.transform.SetSiblingIndex(newSiblingIndex);
        index = newSiblingIndex;
    }

    public void ChangeEnum(Slot slot)
    {
        typeOfCard = slot;
    }

    public void DisableHighlight()
    {
        highlightBorder.SetActive(false);
    }
}
