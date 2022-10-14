using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ThisChoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject fullCardImage = null;

    public List<Choice> thisChoice = new List<Choice>();
    public int thisId;

    public int id;
    public string cardName;
    public string cardDescription;
    private Color frameColor;
    public Sprite thisSprite;
    public int tier;
    public int cost;

    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text tierText;

    public Image thatImage;
    public Image fullSplash;

    public Image frame;
    public GameObject highlightBorder;

    void Awake()
    {
        thisChoice[0] = ChoiceDB.choiceList[thisId];

        id = thisChoice[0].id;
        cardName = thisChoice[0].cardName;
        cardDescription = thisChoice[0].cardDescription;
        thisSprite = thisChoice[0].thisImage;
        tier = thisChoice[0].tier;
        cost = thisChoice[0].cost;
    }

    void Start()
    {
        // set the color of the card
        if (thisChoice[0].color == "Red")
        {
            frame.color = new Color32(255, 0, 0, 255);
            //fullCardImage.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
        }
        if (thisChoice[0].color == "Blue")
            frame.color = new Color32(0, 55, 255, 255);
        if (thisChoice[0].color == "Green")
            frame.color = new Color32(0, 255, 0, 255);
        if (thisChoice[0].color == "Black")
            frame.color = new Color32(40, 40, 40, 255);
        if (thisChoice[0].color == "Grey")
            frame.color = new Color32(150, 150, 150, 255);
        if (thisChoice[0].color == "White")
            frame.color = new Color32(255, 255, 255, 255);

        fullCardImage.GetComponent<Image>().color = frame.color;

        fullCardImage.SetActive(false);
    }

    void Update()
    {
        if (nameText != null && descriptionText != null && tierText != null)
        {
            descriptionText.text = cardDescription;
            nameText.text = cardName;
            tierText.text = "" + tier;

            thatImage.sprite = thisSprite;
            fullSplash.sprite = thisSprite;
        }
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
