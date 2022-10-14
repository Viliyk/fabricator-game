using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ThisItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject fullCardImage = null;

    public List<Item> thisItem = new List<Item>();
    public int thisId;

    public int id;
    public string cardName;
    public string cardDescription;
    private Color frameColor;
    public Sprite thisSprite;
    public int tier;
    public int cost;
    public float energyCost;
    public float timeCost;

    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text tierText;
    public TMP_Text energyCostText;
    public TMP_Text timeCostText;
    public TMP_Text buildCooldownText;

    public Image thatImage;
    public Image fullSplash;

    public Image frame;
    public GameObject highlightBorder;
    public GameObject targetBorder;
    public Slider stasisSlider;
    public GameObject stasisGauge;

    public GameObject buildingIndicator;

    public float baseTimeCost;

    public float speed;

    void Awake()
    {
        thisItem[0] = ItemDB.itemList[thisId];

        id = thisItem[0].id;
        cardName = thisItem[0].cardName;
        cardDescription = thisItem[0].cardDescription;
        thisSprite = thisItem[0].thisImage;
        tier = thisItem[0].tier;
        cost = thisItem[0].cost;
        energyCost = thisItem[0].energyCost;
        timeCost = thisItem[0].timeCost;

        stasisSlider.maxValue = timeCost;
    }

    void Start()
    {
        baseTimeCost = timeCost;

        timeCostText.text = timeCost.ToString("F1") + "s";

        // set the color of the card
        if (thisItem[0].color == "Red")
        {
            frame.color = new Color32(255, 0, 0, 255);
            //fullCardImage.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
        }
        if (thisItem[0].color == "Blue")
            frame.color = new Color32(0, 55, 255, 255);
        if (thisItem[0].color == "Green")
            frame.color = new Color32(0, 255, 0, 255);
        if (thisItem[0].color == "Black")
            frame.color = new Color32(40, 40, 40, 255);
        if (thisItem[0].color == "Grey")
            frame.color = new Color32(150, 150, 150, 255);
        if (thisItem[0].color == "White")
            frame.color = new Color32(255, 255, 255, 255);

        fullCardImage.GetComponent<Image>().color = frame.color;

        fullCardImage.SetActive(false);
    }

    void Update()
    {
        stasisSlider.value = timeCost;

        if (nameText != null && descriptionText != null && tierText != null)
        {
            descriptionText.text = cardDescription;
            nameText.text = cardName;
            tierText.text = "" + tier;
            energyCostText.text = "" + energyCost;
            //timeCostText.text = timeCost.ToString("F1");
            buildCooldownText.text = timeCost.ToString("F1") + "s";

            thatImage.sprite = thisSprite;
            fullSplash.sprite = thisSprite;
        }
    }

    public void ActivateStasisSlider()
    {
        stasisSlider.gameObject.SetActive(true);
        stasisGauge.gameObject.SetActive(true);
    }

    public void DeactivateStasisSlider()
    {
        stasisSlider.gameObject.SetActive(false);
        stasisGauge.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)      // activate card image
    {
        if (eventData.pointerDrag == null)
            fullCardImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)       // deactivate card image
    {
        fullCardImage.SetActive(false);
        //fullCardImage.transform.SetParent(transform);
    }
}

