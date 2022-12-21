using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ThisCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDropHandler
{
    [SerializeField] private GameObject fullCardImage = null;

    public List<Card> thisCard = new List<Card>();
    public int thisId;

    public int id;
    public string cardName;
    public string cardDescription;
    public int tier;
    public int techLevel;
    public int cost;
    public float energyCost;
    public bool station;
    public int attack;
    public int health;
    public float buildTime;
    public float baseBuildTime;
    private int baseHealth;
    private Color frameColor;
    public string type;
    public bool structure;
    public bool golden;
    public bool admission;
    public bool shield;
    public bool cleave;
    public bool guard;
    public bool vengeance;
    public bool command;
    public bool rapidFire;
    public List<List<int>> abilityList = new List<List<int>>();

    public bool thisIsTargeted = false;

    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text tierText;
    public TMP_Text attackText;
    public TMP_Text healthText;
    public TMP_Text attackText2;
    public TMP_Text healthText2;
    public TMP_Text cooldownText;
    public TMP_Text typeText;
    public TMP_Text burnText;
    public TMP_Text energyCostText;
    public TMP_Text timeCostText;
    public TMP_Text buildCooldownText;
    public TMP_Text techLevelText;

    public Sprite thisSprite;
    public Image thatImage;
    public Image fullSplash;

    public Image frame;
    public GameObject highlightBorder;
    public GameObject targetBorder;
    public GameObject barrierIcon;
    public GameObject tauntIcon;
    public Slider chargeSlider;
    public Slider stasisSlider;
    public GameObject stasisGauge;


    public StatBuffs statbuffs;

    public GameObject buildingIndicator;

    public Image bullet;

    public GameObject burnIcon;
    public int burn;

    public bool copy;
    public bool isEnemy;
    public bool onField;

    public bool backline;

    public bool viableTarget;

    public float baseTimeCost;

    public float currentEnergy;

    public float speed;


    public bool bought = false;
    public bool onHold = false;


    void Awake()
    {
        if (!copy)
        {
            thisCard[0] = CardDB.cardList[thisId];

            statbuffs = GetComponent<StatBuffs>();

            id = thisCard[0].id;
            cardName = thisCard[0].cardName;
            cardDescription = thisCard[0].cardDescription;
            tier = thisCard[0].tier;
            techLevel = thisCard[0].techLevel;
            cost = thisCard[0].cost;
            energyCost = thisCard[0].energyCost;
            station = thisCard[0].station;
            attack = thisCard[0].attack;
            health = thisCard[0].health;
            buildTime = thisCard[0].buildTime;
            type = thisCard[0].type;
            thisSprite = thisCard[0].thisImage;

            abilityList = thisCard[0].abilityList;
        }
    }

    void Start()
    {
        // *** chimera egg stopped working when moving these to awake, probably because of RestorePlayerBoard. enemy barriers still worked ***


        //if (!copy)
        //{
        //    thisCard[0] = CardDB.cardList[thisId];

        //    statbuffs = GetComponent<StatBuffs>();

        //    id = thisCard[0].id;
        //    cardName = thisCard[0].cardName;
        //    cardDescription = thisCard[0].cardDescription;
        //    tier = thisCard[0].tier;
        //    cost = thisCard[0].cost;
        //    energyCost = thisCard[0].energyCost;
        //    station = thisCard[0].station;
        //    attack = thisCard[0].attack;
        //    health = thisCard[0].health;
        //    cooldown = thisCard[0].cooldown;
        //    type = thisCard[0].type;
        //    thisSprite = thisCard[0].thisImage;

        //    abilityList = thisCard[0].abilityList;
        //}




        baseHealth = health;
        baseBuildTime = buildTime;
        currentEnergy = 0;

        if (!copy)
        {
            // set the color of the card
            if (thisCard[0].color == "Red")
                frame.color = new Color32(155, 0, 0, 255);
            if (thisCard[0].color == "Blue")
                frame.color = new Color32(0, 28, 129, 255);
            if (thisCard[0].color == "Green")
                frame.color = new Color32(0, 125, 0, 255);
            if (thisCard[0].color == "Black")
                frame.color = new Color32(20, 20, 20, 255);
            if (thisCard[0].color == "Grey")
                frame.color = new Color32(68, 68, 68, 255);
            if (thisCard[0].color == "White")
                frame.color = new Color32(255, 255, 255, 255);

            if (golden == true)
                frame.color = new Color32(255, 215, 0, 255);

            fullCardImage.GetComponent<Image>().color = frame.color;
        }

        fullCardImage.SetActive(false);
    }

    void Update()
    {
        chargeSlider.maxValue = baseBuildTime;                     // set charge bar
        chargeSlider.value = baseBuildTime - buildTime;


        stasisSlider.maxValue = energyCost;
        stasisSlider.value = currentEnergy;


        if (burn > 0)                                                   // set burn icon
            burnIcon.SetActive(true);
        else if (burn <= 0)
            burnIcon.SetActive(false);


        if (nameText != null && descriptionText != null && tierText != null)
        {
            if (CheckForAbility(1))             // add barrier icon when shield is true
                barrierIcon.SetActive(true);
            else
                barrierIcon.SetActive(false);

            if (CheckForAbility(2))              // add taunt icon when taunt is true
                tauntIcon.SetActive(true);
            else
                tauntIcon.SetActive(false);

            cooldownText.text = baseBuildTime.ToString("F1") + "s";
            descriptionText.text = cardDescription;
            nameText.text = cardName;
            tierText.text = "" + tier;
            techLevelText.text = "" + techLevel;
            attackText.text = "" + attack;
            healthText.text = "" + health;
            attackText2.text = "" + attack;
            healthText2.text = "" + health;
            burnText.text = "" + burn;
            energyCostText.text = "" + energyCost;
            //timeCostText.text = timeCost.ToString("F1");
            buildCooldownText.text = currentEnergy.ToString("F0") + "/" + energyCost.ToString("F0");
            typeText.text = type;

            thatImage.sprite = thisSprite;
            fullSplash.sprite = thisSprite;
        }

        // no viable targets should exist when target mode is not true
        if (GlobalControl.Instance.targetMode == false)
        {
            viableTarget = false;
            //DeactivateTargetBorder();
        }
        // show target border when this is a viable target
        if (viableTarget)
            ActivateTargetBorder();
    }

    bool CheckForAbility(int abilityId)
    {
        for (int i = 0; i < abilityList.Count; i++)
        {
            if (abilityList[i][0] == abilityId)
                return true;
        }

        return false;
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

    public void ActivateTargetBorder()
    {
        targetBorder.SetActive(true);
    }

    public void DeactivateTargetBorder()
    {
        targetBorder.SetActive(false);
    }

    public void TargetThis()
    {
        thisIsTargeted = true;
    }

    public void ChangeHealthToRed()
    {
        healthText.color = new Color32(255, 0, 0, 255);
    }

    public void OnPointerEnter(PointerEventData eventData)      // activate card image
    {
        if (eventData.pointerDrag == null)
            fullCardImage.SetActive(true);

        if (Input.GetMouseButtonUp(0) && GlobalControl.Instance.targetMode)
            print(id);
    }

    public void OnPointerExit(PointerEventData eventData)       // deactivate card image
    {
        fullCardImage.SetActive(false);
        //fullCardImage.transform.SetParent(transform);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BattleManager battleManager = GlobalControl.Instance.battleManager;

        if (battleManager != null)
        {
            if (bought)
                onHold = !onHold;
            else if (!bought && techLevel <= battleManager.techLevel && battleManager.energy >= energyCost)
            {
                battleManager.PayEnergy(energyCost, false);
                DeactivateTargetBorder();
                bought = true;
            }

            if (GlobalControl.Instance.targetMode && viableTarget)
            {
                //ShopManager shopManager = GlobalControl.Instance.shopManager;

                battleManager.ActivateMinion(null, null, this);

                GlobalControl.Instance.targetMode = false;
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        ShopManager shopManager = GlobalControl.Instance.shopManager;
        BattleManager battleManager = GlobalControl.Instance.battleManager;
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        ThisConsumable c = eventData.pointerDrag.GetComponent<ThisConsumable>();
        ThisItem i = eventData.pointerDrag.GetComponent<ThisItem>();

        if (c != null)
        {
            if (shopManager != null)
                shopManager.ConsumableFunctionality(c.id);

            if (battleManager != null)
                battleManager.ConsumableFunctionality(c, this);

            d.EndDrag();
        }

        if (i != null)
        {
            if (battleManager != null)
                battleManager.ItemFunctionality(i, this);

            d.EndDrag();
        }
    }
}
