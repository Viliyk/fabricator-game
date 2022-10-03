using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ThisConsumable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject fullCardImage = null;
    [SerializeField] private GameObject soldOutImage = null;

    public List<Consumable> thisConsumable = new List<Consumable>();
    public int thisId;

    public int id;
    public string cardName;
    public string cardDescription;
    public int tier;
    public string type;
    private Color frameColor;

    public TMP_Text nameText;
    public TMP_Text splashName;
    public TMP_Text descriptionText;
    public TMP_Text rarityText;
    public TMP_Text typeText;

    public Sprite thisSprite;
    public Image thatImage;
    public Image fullSplash;

    public Image frame;
    public GameObject highlightBorder;

    public bool inShop = false;
    public bool clicked = false;
    public bool soldOut = false;

    void Awake()
    {
        thisConsumable[0] = ConsumableDB.consumableList[thisId];

        id = thisConsumable[0].id;
        cardName = thisConsumable[0].cardName;
        cardDescription = thisConsumable[0].cardDescription;
        tier = thisConsumable[0].tier;
        type = thisConsumable[0].type;
        thisSprite = thisConsumable[0].thisImage;
    }

    void Start()
    {
        // set the color of the card
        if (thisConsumable[0].color == "Red")
        {
            frame.color = new Color32(245, 112, 0, 255);
            //fullCardImage.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
        }
        if (thisConsumable[0].color == "Blue")
            frame.color = new Color32(138, 46, 192, 255);
        if (thisConsumable[0].color == "Green")
            frame.color = new Color32(0, 255, 0, 255);
        if (thisConsumable[0].color == "Black")
            frame.color = new Color32(40, 40, 40, 255);
        if (thisConsumable[0].color == "Grey")
            frame.color = new Color32(150, 150, 150, 255);
        if (thisConsumable[0].color == "White")
            frame.color = new Color32(255, 255, 255, 255);
    }

    void Update()
    {
        if (nameText != null && descriptionText != null && rarityText != null)
        {
            descriptionText.text = cardDescription;
            nameText.text = cardName;
            rarityText.text = "" + tier;
            typeText.text = type;
            thatImage.sprite = thisSprite;
            fullSplash.sprite = thisSprite;
            splashName.text = cardName;
        }
    }

    public void BuyThisConsumable()
    {
        soldOut = true;
        soldOutImage.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inShop == true && soldOut == false)
            clicked = true;
    }

    public void OnPointerEnter(PointerEventData eventData)      // activate card image
    {
        fullCardImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)       // deactivate card image
    {
        fullCardImage.SetActive(false);
        //fullCardImage.transform.SetParent(transform);
    }
}