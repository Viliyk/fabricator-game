using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ThisRelic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject fullCardImage = null;
    [SerializeField] private GameObject targetBorder = null;
    [SerializeField] private GameObject soldOutImage = null;

    public List<Relic> thisRelic = new List<Relic>();
    public int thisId;

    public int id;
    public string relicName;
    public string relicDescription;
    public int tier;
    public string type;
    private Color frameColor;
    public bool active;
    public int cost;

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

    public bool thisIsTargeted = false;
    public bool cooldown = false;
    public bool inShop = false;
    public bool clicked = false;
    public bool soldOut = false;

    void Awake()
    {
        thisRelic[0] = RelicDB.relicList[thisId];

        id = thisRelic[0].id;
        relicName = thisRelic[0].relicName;
        relicDescription = thisRelic[0].relicDescription;
        tier = thisRelic[0].tier;
        type = thisRelic[0].type;
        thisSprite = thisRelic[0].thisImage;
        active = thisRelic[0].active;
        cost = thisRelic[0].cost;
    }

    void Start()
    {
        // set the color of the card
        if (thisRelic[0].color == "Red")
        {
            frame.color = new Color32(245, 112, 0, 255);
            //fullCardImage.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
        }
        if (thisRelic[0].color == "Blue")
            frame.color = new Color32(138, 46, 192, 255);
        if (thisRelic[0].color == "Green")
            frame.color = new Color32(0, 255, 0, 255);
        if (thisRelic[0].color == "Black")
            frame.color = new Color32(40, 40, 40, 255);
        if (thisRelic[0].color == "Grey")
            frame.color = new Color32(150, 150, 150, 255);
        if (thisRelic[0].color == "White")
            frame.color = new Color32(255, 255, 255, 255);


    }

    void Update()
    {
        if (nameText != null && descriptionText != null && rarityText != null)
        {
            descriptionText.text = relicDescription;
            nameText.text = relicName;
            rarityText.text = "" + tier;
            typeText.text = type;
            thatImage.sprite = thisSprite;
            fullSplash.sprite = thisSprite;
            splashName.text = relicName;
        }

        if (active == true && cooldown == false)
            targetBorder.SetActive(true);
        else
            targetBorder.SetActive(false);
    }

    public void BuyThisRelic()
    {
        GlobalControl.Instance.ownedRelics.Add(id);

        soldOut = true;
        soldOutImage.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inShop == true && soldOut == false)
            clicked = true;


        if (active == true && cooldown == false)
            thisIsTargeted = true;
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
