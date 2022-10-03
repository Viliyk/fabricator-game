using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject cardTemplate = null;
    [SerializeField] private GameObject itemTemplate = null;
    [SerializeField] private GameObject consumableTemplate = null;
    [SerializeField] private GameObject relicTemplate = null;
    [SerializeField] private GameObject choiceTemplate = null;
    [SerializeField] private GameObject shop = null;
    [SerializeField] private GameObject board = null;
    [SerializeField] private GameObject hand = null;
    [SerializeField] private GameObject relics = null;
    [SerializeField] private GameObject buyZone = null;
    [SerializeField] private GameObject shopHighlight = null;
    [SerializeField] private GameObject boardHighlight = null;
    [SerializeField] private GameObject choiceScreen = null;
    [SerializeField] private ChoicePanel choicePanel = null;
    [SerializeField] private MapManager mapManager = null;
    [SerializeField] private TMP_Text levelText = null;
    [SerializeField] private TMP_Text upgradeCostText = null;
    [SerializeField] private TMP_Text goldText = null;
    [SerializeField] private TMP_Text startingGoldText = null;
    [SerializeField] private TMP_Text lockText = null;
    [SerializeField] private TMP_Text turnText = null;
    [SerializeField] private TMP_Text livesText = null;
    [SerializeField] private TMP_Text creditsText = null;
    [SerializeField] private GameObject tradingHubUI = null;


    //[SerializeField] private GameObject reticle = null;


    private List<ThisCard> boardMinions = new List<ThisCard>();
    private List<ThisCard> handMinions = new List<ThisCard>();
    private List<ThisCard> playerMinions = new List<ThisCard>();

    private ThisCard createdCard;
    private GameObject spawnedCard;
    private ThisItem createdItem;
    private GameObject spawnedItem;
    private ThisConsumable createdConsumable;
    private GameObject spawnedConsumable;
    private ThisRelic createdRelic;
    private GameObject spawnedRelic;
    private ThisChoice createdChoice;
    private GameObject spawnedChoice;


    private StatBuffs statbuffs;
    public int gold;
    private int cardsInShop = 0;
    private List<int> availableShopCards = new List<int>();
    private List<int> availableShopItems = new List<int>();

    // targeted admission variables
    private ThisCard targetedAdmissionCard = null;
    private bool checkForClick = false;
    private bool sentinelWasTargeted = false;
    private bool chimeraWasTargeted = false;
    private bool defectWasTargeted = false;
    private bool compoundWasTargeted = false;
    private bool animaWasTargeted = false;

    private int targetedConsumable;
    private bool consumableClick = false;

    // starting values of loaded variables are set in GlobalControl
    public int shopLevel;
    public int turnNumber;
    public int lives;
    public int credits;
    private int startingGold;
    private int upgradeCost;
    private int nextUpgrade;
    private int[] yourCards;
    private int[] handCards;
    private int[] lockedCards;
    private bool isLocked;

    void Start()
    {
        GlobalControl.Instance.shopManager = this;
        GlobalControl.Instance.targetMode = false;

        LoadData();     // load values from GlobalControl
        SetAvailableShopCards();   // set tier limiter according to shop level
        turnNumber = GlobalControl.Instance.turnNumber;           // count turns
        RestorePlayerBoard();   // spawn player's board back
        RestorePlayerHand();    // spawn player's hand back
        RestorePlayerRelics();  // spawn player's relics back

        if (isLocked == true)   // spawn locked cards and count them
        {
            foreach (int unit in lockedCards)
            {
                if (unit != 0)
                    SpawnShopCard(unit, false);
            }
            cardsInShop = shop.transform.childCount;
        }

        //DoReroll(0);    // spawn initial shop cards

        if (startingGold < 10)      // increase starting gold
            startingGold++;

        if (upgradeCost > 0)        // reduce upgrade cost
            upgradeCost--;

        isLocked = false;           // unlock shop

        //mapManager.gameObject.SetActive(true);      // set map active

        FillChoices();
    }

    // text elements and automatic restart, targeted admission check
    void Update()
    {
        // set all the text elements to follow their variables
        if (shopLevel == 6)
            upgradeCostText.text = " ";
        else
            upgradeCostText.text = "(" + upgradeCost + ")";

        goldText.text = gold + " /";
        startingGoldText.text = startingGold + " g";
        levelText.text = "" + shopLevel;
        turnText.text = "Turn " + turnNumber;
        livesText.text = "Lives: " + lives;
        creditsText.text = "Credits: " + credits;

        // restart game when out of lives
        if (lives <= 0)
            RestartButton();

        // targeted admission check
        if (checkForClick == true)
        {
            ThisCard[] cards = board.transform.GetComponentsInChildren<ThisCard>();
            foreach (ThisCard card in cards)
            {
                if (card.thisIsTargeted == true)
                    AdmissionFunctionality(targetedAdmissionCard);
            }
        }

        // consumable click check
        if (consumableClick == true)
        {
            ThisCard[] cards = board.transform.GetComponentsInChildren<ThisCard>();
            foreach (ThisCard card in cards)
            {
                if (card.thisIsTargeted == true)
                    ConsumableFunctionality(targetedConsumable);
            }
        }

        // relic click check
        ThisRelic[] activeRelics = relics.transform.GetComponentsInChildren<ThisRelic>();
        foreach (ThisRelic relic in activeRelics)
        {
            if (relic.thisIsTargeted == true)
            {
                relic.thisIsTargeted = false;
                ActiveRelicFunctionality(relic);
            }
        }
    }

    void FillChoices()
    {
        foreach (Transform child in shop.transform)
        {
            Destroy(child.gameObject);
        }

        if (turnNumber == 1)
        {
            SpawnChoice(2);
            return;
        }

        SpawnChoice(1);
        SpawnChoice(2);
        SpawnChoice(3);
    }

    void SpawnChoice(int id)
    {
        createdChoice = choiceTemplate.GetComponent<ThisChoice>();
        createdChoice.thisId = id;
        spawnedChoice = Instantiate(choiceTemplate, new Vector3(0, 0, 0), Quaternion.identity);
        spawnedChoice.transform.SetParent(shop.transform, false);
        spawnedChoice.GetComponent<Draggable>().ChangeEnum(Draggable.Slot.CHOICE);
    }

    public void ActivateChoice(ThisChoice choice)
    {
        if (gold < choice.cost)
            return;

        switch (choice.thisId)
        {
            default:
                break;

            case 1:
                {
                    lives += 10;
                    Destroy(choice.gameObject);
                }
                break;
            case 2:
                {
                    tradingHubUI.SetActive(true);
                    DoReroll(0, true);
                    Destroy(choice.gameObject);
                }
                break;
            case 3:
                {
                    gold += 3;
                    LoadBattleScene();
                }
                break;
        }

        gold -= choice.cost;
    }

    public void StartOfTurnEffects()
    {
        ThisCard[] cards = board.transform.GetComponentsInChildren<ThisCard>();

        foreach (ThisCard card in cards)
        {
            if (card.id == 32)      // resupplier
            {
                SpawnConsumable(4);
                if (card.golden)
                    SpawnConsumable(4);
            }
        }

        if (GlobalControl.Instance.ownedRelics.Contains(1))     // gold fountain
            SpawnConsumable(1);
    }

    void GetYourMinions()
    {
        boardMinions.Clear();
        foreach (Transform card in board.transform)
        {
            ThisCard unit = card.GetComponent<ThisCard>();
            if (unit != null)
                boardMinions.Add(unit);
        }

        handMinions.Clear();
        foreach (Transform card in hand.transform)
        {
            ThisCard unit = card.GetComponent<ThisCard>();
            if (unit != null)
                handMinions.Add(unit);
        }

        playerMinions.Clear();
        playerMinions.AddRange(boardMinions);
        playerMinions.AddRange(handMinions);
    }

    public void TriggerOnPlay(ThisCard playedCard)
    {
        TriggerByType(playedCard);

        //if (playedCard.admission == true)
        AdmissionFunctionality(playedCard);

        if (playedCard.golden == true)
        {
            SpawnConsumable(6);
            //choicePanel.SetPoolTriple(Mathf.Clamp(shopLevel + 1, 2, 5));
            //choiceScreen.SetActive(true);
            //choicePanel.SpawnCard();
        }
    }

    void TriggerByType(ThisCard playedCard)
    {
        int goldenMultiplier = 1;

        ThisCard[] cards = board.transform.GetComponentsInChildren<ThisCard>();
        List<ThisCard> chimeras = new List<ThisCard>();
        List<ThisCard> sentinels = new List<ThisCard>();
        List<ThisCard> defects = new List<ThisCard>();
        foreach (ThisCard card in cards)
        {
            if (card.type == "Chimera")
                chimeras.Add(card);
            if (card.type == "Sentinel")
                sentinels.Add(card);
            if (card.type == "Defect")
                defects.Add(card);
        }

        if (playedCard.type == "Sentinel")
        {
            foreach (ThisCard card in cards)
            {
                if (card.id == 22 && card != playedCard)    // janitor
                {
                    if (card.golden == true)
                        goldenMultiplier = 2;
                    else
                        goldenMultiplier = 1;

                    sentinels.Remove(card);     // remove the buff giver from the list so it cant buff itself
                    ThisCard target = sentinels[Random.Range(0, sentinels.Count)];
                    target.attack += 1 * goldenMultiplier;
                    target.health += 1 * goldenMultiplier;
                    sentinels.Add(card);        // add the buff giver back so another card with the same id can buff it
                }
            }
        }

        if (playedCard.type == "Defect")
        {
            foreach (ThisCard card in cards)
            {
                if (card.id == 31 && card != playedCard)    // writhing mass
                {
                    if (card.golden == true)
                        goldenMultiplier = 2;
                    else
                        goldenMultiplier = 1;

                    card.attack += 1 * goldenMultiplier;
                    card.health += 1 * goldenMultiplier;
                }
            }
        }

        if (playedCard.admission == true)
        {
            foreach (ThisCard card in cards)
            {
                if (card.id == 29 && card != playedCard)    // goose
                {
                    if (card.golden == true)
                        goldenMultiplier = 2;
                    else
                        goldenMultiplier = 1;

                    foreach (ThisCard defect in defects)
                    {
                        defect.attack += 1 * goldenMultiplier;
                        defect.health += 1 * goldenMultiplier;
                    }
                }
            }
        }
    }

    public void ConsumableFunctionality(int id)
    {
        ThisCard[] cards = board.transform.GetComponentsInChildren<ThisCard>();

        switch (id)
        {
            default:
                print("Error: Invalid consumable id");
                break;

            case 1:     // coin
                gold++;
                break;
            case 2:     // wide buff
                {
                    foreach (ThisCard card in cards)
                    {
                        card.attack++;
                        card.health++;
                    }
                }
                break;
            case 3:     // guard potion
                {
                    targetedConsumable = id;

                    foreach (ThisCard card in cards)
                    {
                        if (card.thisIsTargeted == true)
                        {
                            foreach (ThisCard a in cards)
                                a.DeactivateTargetBorder();

                            card.guard = true;
                            card.thisIsTargeted = false;
                            consumableClick = false;
                            return;
                        }
                    }
                    foreach (ThisCard card in cards)
                        card.ActivateTargetBorder();
                    consumableClick = true;
                }
                break;
            case 4:     // banana
                {
                    targetedConsumable = id;

                    foreach (ThisCard card in cards)
                    {
                        if (card.thisIsTargeted == true)
                        {
                            foreach (ThisCard a in cards)
                                a.DeactivateTargetBorder();

                            card.attack++;
                            card.health++;
                            card.thisIsTargeted = false;
                            consumableClick = false;
                            return;
                        }
                    }
                    foreach (ThisCard card in cards)
                        card.ActivateTargetBorder();
                    consumableClick = true;
                }
                break;
            case 6:     // golden ticket
                choicePanel.SetPoolTriple(Mathf.Clamp(shopLevel + 1, 2, 6));
                break;
            case 7:     // borger
                {
                    targetedConsumable = id;

                    foreach (ThisCard card in cards)
                    {
                        if (card.thisIsTargeted == true)
                        {
                            foreach (ThisCard a in cards)
                                a.DeactivateTargetBorder();

                            // give chonk an extra +1/+1
                            if (card.id == 30)
                            {
                                card.attack++;
                                card.health++;
                            }

                            card.attack++;
                            card.health++;
                            card.thisIsTargeted = false;
                            consumableClick = false;
                            return;
                        }
                    }
                    foreach (ThisCard card in cards)
                    {
                        if (card.type == "Chimera")
                            card.ActivateTargetBorder();
                    }
                    consumableClick = true;
                }
                break;
        }
    }

    public void ActiveRelicFunctionality(ThisRelic thisRelic)
    {
        switch (thisRelic.id)
        {
            default:
                print("Error: Invalid relic id");
                break;

            case 5:     // diamond hands
                {
                    if (gold > 0 && hand.transform.childCount < 10)
                    {
                        gold--;
                        SpawnConsumable(1);
                        //thisRelic.cooldown = true;
                    }
                    break;
                }
            case 6:     // meat cube
                {
                    if (hand.transform.childCount < 1)
                        return;

                    thisRelic.cooldown = true;

                    int numOfCards = 0;
                    foreach (Transform card in hand.transform)
                    {
                        numOfCards++;
                        Destroy(card.gameObject);
                    }
                    for (int i = 0; i < numOfCards; i++)
                        SpawnConsumable(7);

                    break;
                }
        }
    }

    void AdmissionFunctionality(ThisCard playedCard)
    {
        ThisCard[] cards = board.transform.GetComponentsInChildren<ThisCard>();

        int goldenMultiplier = 1;           // if the played card is golden this automatically scales effects up 2*
        if (playedCard.golden == true)
            goldenMultiplier = 2;


        for (int i = 0; i < playedCard.abilityList.Count; i++)
        {
            switch (playedCard.abilityList[i][0])
            {
                default:
                    break;

                case 13:    // slime colony
                    {
                        SpawnPlayerCard(19, false);
                    }
                    break;
                case 14:    // junkbot
                    {
                        foreach (ThisCard card in cards)
                        {
                            if (card.type == "Defect" && card != playedCard)
                                card.health += playedCard.abilityList[i][1];
                        }
                    }
                    break;
            }
        }


        //switch (playedCard.id)
        //{
        //    default:
        //        print("Error: This card doesn't have admission");
        //        break;

        //    case 1:     // slime colony
        //    case 7:     // infested slime
        //        {
        //            ThisCard summonedCard;
        //            //GameObject spawnedReticle;

        //            summonedCard = SpawnPlayerCard(19, false);
        //            summonedCard.attack *= goldenMultiplier;
        //            summonedCard.health *= goldenMultiplier;
        //            if (playedCard.golden == true)
        //                summonedCard.golden = true;

        //            //spawnedReticle = Instantiate(reticle, new Vector3(0, 0, 0), Quaternion.identity);
        //            //spawnedReticle.transform.SetParent(board.transform.parent);
        //        }
        //        break;
        //    case 2:     // malignant matter
        //        { lives -= 2; }
        //        break;
        //    case 6:     // temp chimera #1
        //        {
        //            foreach (ThisCard card in cards)
        //            {
        //                if (card.type == "Chimera" && card != playedCard)
        //                    card.attack += 2 * goldenMultiplier;
        //            }
        //        }
        //        break;
        //    case 18:    // horrendous growth
        //        {
        //            foreach (ThisCard card in cards)
        //            {
        //                if (card != playedCard)
        //                {
        //                    playedCard.attack += card.attack * goldenMultiplier;
        //                    playedCard.health += card.health * goldenMultiplier;
        //                    Destroy(card.gameObject);
        //                }
        //            }
        //        }
        //        break;
        //    case 5:     // junkbot
        //        {
        //            foreach (ThisCard card in cards)
        //            {
        //                if (card.type == "Defect" && card != playedCard)
        //                    card.health += 1 * goldenMultiplier;
        //            }
        //        }
        //        break;
        //    case 12:    // temp defect #4
        //        {
        //            foreach (ThisCard card in cards)
        //            {
        //                if (card.type == "Defect" && card != playedCard)
        //                {
        //                    card.attack += 4 * goldenMultiplier;
        //                }
        //            }
        //        }
        //        break;
        //    case 3:     // sentinel scout
        //        {
        //            targetedAdmissionCard = playedCard;

        //            foreach (ThisCard card in cards)
        //            {
        //                if (card.thisIsTargeted == true)
        //                {
        //                    card.attack += 2 * goldenMultiplier;
        //                    card.health += 2 * goldenMultiplier;
        //                    card.thisIsTargeted = false;
        //                    sentinelWasTargeted = true;
        //                }
        //            }
        //            foreach (ThisCard card in cards)
        //            {
        //                if (card.type == "Sentinel" && card != playedCard)
        //                {
        //                    if (sentinelWasTargeted == false)
        //                    {
        //                        card.ActivateTargetBorder();
        //                        checkForClick = true;
        //                    }
        //                    else
        //                    {
        //                        card.DeactivateTargetBorder();
        //                        checkForClick = false;
        //                    }
        //                }
        //            }
        //            if (sentinelWasTargeted == true)
        //                sentinelWasTargeted = false;
        //        }
        //        break;
        //    case 23:    // spare-part vendor
        //        {
        //            bool sentinelWasFound = false;
        //            bool chimeraWasFound = false;
        //            bool defectWasFound = false;

        //            targetedAdmissionCard = playedCard;

        //            foreach (ThisCard card in cards)
        //            {
        //                if (card.thisIsTargeted == true)
        //                {
        //                    card.attack += 1 * goldenMultiplier;
        //                    card.health += 1 * goldenMultiplier;
        //                    card.thisIsTargeted = false;
        //                    if (card.type == "Sentinel")
        //                        sentinelWasTargeted = true;
        //                    if (card.type == "Chimera")
        //                        chimeraWasTargeted = true;
        //                    if (card.type == "Defect")
        //                        defectWasTargeted = true;
        //                }
        //            }
        //            foreach (ThisCard card in cards)
        //            {
        //                if (card.type == "Sentinel" && card != playedCard)
        //                {
        //                    if (sentinelWasTargeted == false)
        //                    {
        //                        card.ActivateTargetBorder();
        //                        sentinelWasFound = true;
        //                        checkForClick = true;
        //                    }
        //                    else
        //                        card.DeactivateTargetBorder();
        //                }
        //                if (card.type == "Chimera" && card != playedCard)
        //                {
        //                    if (chimeraWasTargeted == false)
        //                    {
        //                        card.ActivateTargetBorder();
        //                        chimeraWasFound = true;
        //                        checkForClick = true;
        //                    }
        //                    else
        //                        card.DeactivateTargetBorder();
        //                }
        //                if (card.type == "Defect" && card != playedCard)
        //                {
        //                    if (defectWasTargeted == false)
        //                    {
        //                        card.ActivateTargetBorder();
        //                        defectWasFound = true;
        //                        checkForClick = true;
        //                    }
        //                    else
        //                        card.DeactivateTargetBorder();
        //                }
        //            }

        //            if (sentinelWasFound == false)
        //                sentinelWasTargeted = true;
        //            if (chimeraWasFound == false)
        //                chimeraWasTargeted = true;
        //            if (defectWasFound == false)
        //                defectWasTargeted = true;

        //            if (sentinelWasTargeted == true)
        //                sentinelWasFound = false;
        //            if (chimeraWasTargeted == true)
        //                chimeraWasFound = false;
        //            if (defectWasTargeted == true)
        //                defectWasFound = false;

        //            if (sentinelWasFound == false && chimeraWasFound == false && defectWasFound == false)
        //            {
        //                sentinelWasTargeted = false;
        //                chimeraWasTargeted = false;
        //                defectWasTargeted = false;
        //                checkForClick = false;
        //            }
        //        }
        //        break;
        //    case 25:    // cockroach
        //        {
        //            for (int i = 0; i < goldenMultiplier; i++)
        //                SpawnConsumable(7);
        //        }
        //        break;
        //    case 27:    // alchemist
        //        {
        //            //for (int i = 0; i < goldenMultiplier; i++)
        //            choicePanel.SetPoolConsumable(1);
        //            //SpawnConsumable(Random.Range(1, 6));
        //        }
        //        break;
        //}
    }

    void SpawnShopCard(int id, bool isItem)
    {
        if (!isItem)
        {
            createdCard = cardTemplate.GetComponent<ThisCard>();
            createdCard.thisId = id;
            spawnedCard = Instantiate(cardTemplate, new Vector3(0, 0, 0), Quaternion.identity);
            spawnedCard.transform.SetParent(shop.transform, false);
            spawnedCard.GetComponent<Draggable>().ChangeEnum(Draggable.Slot.SHOP);
        }
        else
        {
            createdItem = itemTemplate.GetComponent<ThisItem>();
            createdItem.thisId = id;
            spawnedItem = Instantiate(itemTemplate, new Vector3(0, 0, 0), Quaternion.identity);
            spawnedItem.transform.SetParent(shop.transform, false);
            spawnedItem.GetComponent<Draggable>().ChangeEnum(Draggable.Slot.SHOP);
        }
    }

    public void SpawnConsumable(int id)
    {
        if (hand.transform.childCount < 10)
        {
            createdConsumable = consumableTemplate.GetComponent<ThisConsumable>();
            createdConsumable.thisId = id;
            spawnedConsumable = Instantiate(consumableTemplate, new Vector3(0, 0, 0), Quaternion.identity);
            spawnedConsumable.transform.SetParent(board.transform, false);
            //createdConsumable = spawnedConsumable.GetComponent<ThisConsumable>();
        }
    }

    public void SpawnRelic(int id)
    {
        createdRelic = relicTemplate.GetComponent<ThisRelic>();
        createdRelic.thisId = id;
        spawnedRelic = Instantiate(relicTemplate, new Vector3(0, 0, 0), Quaternion.identity);
        spawnedRelic.transform.SetParent(relics.transform, false);
    }

    public ThisCard SpawnPlayerCard(int id, bool isHandCard)
    {
        if ((isHandCard == false && board.transform.childCount <= 7) || (isHandCard == true && hand.transform.childCount < 10))
        {
            createdCard = cardTemplate.GetComponent<ThisCard>();
            createdCard.thisId = id;
            spawnedCard = Instantiate(cardTemplate, new Vector3(0, 0, 0), Quaternion.identity);
            createdCard = spawnedCard.GetComponent<ThisCard>();
            if (isHandCard == false)
            {
                spawnedCard.transform.SetParent(board.transform, false);
                spawnedCard.GetComponent<Draggable>().ChangeEnum(Draggable.Slot.BOARD);
            }
            else if (isHandCard == true)
            {
                spawnedCard.transform.SetParent(hand.transform, false);
                spawnedCard.GetComponent<Draggable>().ChangeEnum(Draggable.Slot.HAND);
            }
            StartTripleCheck(id);
        }
        return createdCard;
    }

    public ThisItem SpawnPlayerItem(int id)
    {
        if ((board.transform.childCount <= 7))
        {
            createdItem = itemTemplate.GetComponent<ThisItem>();
            createdItem.thisId = id;
            spawnedItem = Instantiate(itemTemplate, new Vector3(0, 0, 0), Quaternion.identity);
            createdItem = spawnedItem.GetComponent<ThisItem>();

            spawnedItem.transform.SetParent(board.transform, false);
            spawnedItem.GetComponent<Draggable>().ChangeEnum(Draggable.Slot.BOARD);
        }
        return createdItem;
    }

    public void StartTripleCheck(int droppedCardId)
    {
        return;




        StartCoroutine(TripleCheck(droppedCardId));
    }

    IEnumerator TripleCheck(int droppedCardId)
    {
        yield return new WaitForSeconds(0.2f);
        GetYourMinions();

        int tripleCount = 0;

        ThisCard goldenCard;
        int goldenCardAttack = 0;
        int goldenCardHealth = 0;
        bool goldenCardAdmission = false;
        bool goldenCardShield = false;
        bool goldenCardCleave = false;
        bool goldenCardGuard = false;
        bool goldenCardVengeance = false;
        bool goldenCardCommand = false;

        foreach (ThisCard card in playerMinions)
        {
            if (droppedCardId == card.thisId && card.golden == false)
                tripleCount++;
        }
        if (tripleCount >= 3)
        {
            foreach (ThisCard duplicate in playerMinions)
            {
                if (droppedCardId == duplicate.thisId && duplicate.golden == false)
                {
                    goldenCardAttack += duplicate.attack;
                    goldenCardHealth += duplicate.health;
                    if (duplicate.admission == true)
                        goldenCardAdmission = true;
                    if (duplicate.shield == true)
                        goldenCardShield = true;
                    if (duplicate.cleave == true)
                        goldenCardCleave = true;
                    if (duplicate.guard == true)
                        goldenCardGuard = true;
                    if (duplicate.vengeance == true)
                        goldenCardVengeance = true;
                    if (duplicate.command == true)
                        goldenCardCommand = true;

                    Destroy(duplicate.gameObject);
                }
            }

            yield return new WaitForSeconds(0.2f);
            int baseAttack = CardDB.cardList[droppedCardId].attack;
            int baseHealth = CardDB.cardList[droppedCardId].health;

            goldenCard = SpawnPlayerCard(droppedCardId, true);
            goldenCard.golden = true;
            goldenCard.attack = goldenCardAttack;
            goldenCard.health = goldenCardHealth;
            goldenCard.attack -= baseAttack;
            goldenCard.health -= baseHealth;

            goldenCard.admission = goldenCardAdmission;
            goldenCard.shield = goldenCardShield;
            goldenCard.cleave = goldenCardCleave;
            goldenCard.guard = goldenCardGuard;
            goldenCard.vengeance = goldenCardVengeance;
            goldenCard.command = goldenCardCommand;
        }
    }

    void SetAvailableShopCards()
    {
        availableShopCards.Clear();
        foreach (KeyValuePair<int, Card> card in CardDB.cardList)
        {
            if (card.Value.tier <= shopLevel && card.Value.tier > 0)
                availableShopCards.Add(card.Value.id);
        }

        availableShopItems.Clear();
        foreach (KeyValuePair<int, Item> item in ItemDB.itemList)
        {
            if (item.Value.tier <= shopLevel && item.Value.tier > 0)
                availableShopItems.Add(item.Value.id);
        }
    }

    void SaveData()
    {
        GlobalControl.Instance.EmptyContainers();       // empty containers

        foreach (Transform a in board.transform)        // save stats of your board cards
        {
            ThisCard card = a.GetComponent<ThisCard>();
            ThisItem item = a.GetComponent<ThisItem>();

            if (card != null)
            {
                GlobalControl.Instance.newYourCards.Add(Container.SetValues
                    (GlobalControl.Instance.gameObject, card.cost, card.attack, card.health, card.golden, card.admission, card.shield,
                    card.cleave, card.guard, card.vengeance, card.command));
            }
            else if (item != null)                      // save your items
            {
                GlobalControl.Instance.yourItems.Add(item.id);
            }
        }
        foreach (Transform a in hand.transform)
        {
            // save consumables
            ThisConsumable consumable = a.GetComponent<ThisConsumable>();
            if (consumable != null)
            {
                if (consumable.type == "Shop")
                    GlobalControl.Instance.shopConsumables.Add(consumable.id);
                else
                    GlobalControl.Instance.battleConsumables.Add(consumable.id);
            }

            // save stats of your hand cards
            ThisCard card = a.GetComponent<ThisCard>();
            if (card != null)
            {
                GlobalControl.Instance.newHandCards.Add(Container.SetValues
                    (GlobalControl.Instance.gameObject, card.cost, card.attack, card.health, card.golden, card.admission, card.shield,
                    card.cleave, card.guard, card.vengeance, card.command));
            }
        }

        GlobalControl.Instance.shopLevel = shopLevel;           // save variables
        GlobalControl.Instance.gold = gold;
        GlobalControl.Instance.upgradeCost = upgradeCost;
        GlobalControl.Instance.nextUpgrade = nextUpgrade;
        GlobalControl.Instance.isLocked = isLocked;
        GlobalControl.Instance.turnNumber = turnNumber;
        GlobalControl.Instance.lives = lives;
        GlobalControl.Instance.credits = credits;

        for (int i = 0; i < 7; i++)
        {
            GlobalControl.Instance.yourCards[i] = 0;    // clear board card array
        }
        int positionOnBoard = 0;
        foreach (Transform child in board.transform)    // save board cards
        {
            ThisCard card = child.GetComponent<ThisCard>();
            if (card != null)
            {
                GlobalControl.Instance.yourCards[positionOnBoard] = card.thisId;
                positionOnBoard++;
            }
        }

        for (int i = 0; i < 7; i++)
        {
            GlobalControl.Instance.handCards[i] = 0;    // clear hand card array
        }
        int positionOnHand = 0;
        foreach (Transform child in hand.transform)     // save hand cards
        {
            ThisCard card = child.GetComponent<ThisCard>();
            if (card != null)
            {
                GlobalControl.Instance.handCards[positionOnHand] = card.thisId;
                positionOnHand++;
            }
        }

        int positionOnShop = 0;
        foreach (Transform child in shop.transform)     // save locked shop cards
        {
            ThisCard card = child.GetComponent<ThisCard>();

            if (card != null)
            {
                GlobalControl.Instance.lockedCards[positionOnShop] = card.thisId;
                positionOnShop++;
            }
        }
    }

    void LoadData()
    {
        yourCards = GlobalControl.Instance.yourCards;
        handCards = GlobalControl.Instance.handCards;

        GlobalControl.Instance.buyZone = buyZone;
        GlobalControl.Instance.shopHighlight = shopHighlight;
        GlobalControl.Instance.boardHighlight = boardHighlight;

        shopLevel = GlobalControl.Instance.shopLevel;
        gold = GlobalControl.Instance.gold;
        upgradeCost = GlobalControl.Instance.upgradeCost;
        nextUpgrade = GlobalControl.Instance.nextUpgrade;
        isLocked = GlobalControl.Instance.isLocked;
        lockedCards = GlobalControl.Instance.lockedCards;
        turnNumber = GlobalControl.Instance.turnNumber;
        lives = GlobalControl.Instance.lives;
        credits = GlobalControl.Instance.credits;
    }

    void RestorePlayerBoard()
    {
        int i = 0;
        foreach (int unit in yourCards)
        {
            if (unit != 0)
            {
                createdCard = SpawnPlayerCard(unit, false);     // spawn player's cards back to board

                if (GlobalControl.Instance.newYourCards.Count > 0)     // modify their stats to match what they were
                {
                    createdCard.cost = GlobalControl.Instance.newYourCards[i].cost;
                    createdCard.attack = GlobalControl.Instance.newYourCards[i].attack;
                    createdCard.health = GlobalControl.Instance.newYourCards[i].health;
                    createdCard.golden = GlobalControl.Instance.newYourCards[i].golden;
                    createdCard.admission = GlobalControl.Instance.newYourCards[i].admission;
                    createdCard.shield = GlobalControl.Instance.newYourCards[i].shield;
                    createdCard.cleave = GlobalControl.Instance.newYourCards[i].cleave;
                    createdCard.guard = GlobalControl.Instance.newYourCards[i].guard;
                    createdCard.vengeance = GlobalControl.Instance.newYourCards[i].vengeance;
                    createdCard.command = GlobalControl.Instance.newYourCards[i].command;
                }
            }
            i++;
        }
        // restore your items on board
        foreach (int item in GlobalControl.Instance.yourItems)
            SpawnPlayerItem(item);
    }

    void RestorePlayerHand()
    {
        int i = 0;
        foreach (int unit in handCards)
        {
            if (unit != 0)
            {
                createdCard = SpawnPlayerCard(unit, true);      // spawn player's cards back to hand

                if (GlobalControl.Instance.newHandCards[i] != null)     // modify their stats to match what they were
                {
                    createdCard.cost = GlobalControl.Instance.newHandCards[i].cost;
                    createdCard.attack = GlobalControl.Instance.newHandCards[i].attack;
                    createdCard.health = GlobalControl.Instance.newHandCards[i].health;
                    createdCard.golden = GlobalControl.Instance.newHandCards[i].golden;
                    createdCard.admission = GlobalControl.Instance.newHandCards[i].admission;
                    createdCard.shield = GlobalControl.Instance.newHandCards[i].shield;
                    createdCard.cleave = GlobalControl.Instance.newHandCards[i].cleave;
                    createdCard.guard = GlobalControl.Instance.newHandCards[i].guard;
                    createdCard.vengeance = GlobalControl.Instance.newHandCards[i].vengeance;
                    createdCard.command = GlobalControl.Instance.newHandCards[i].command;
                }
            }
            i++;
        }

        foreach (int consumable in GlobalControl.Instance.shopConsumables)      // restore shop consumables
            SpawnConsumable(consumable);
        foreach (int consumable in GlobalControl.Instance.battleConsumables)    // restore battle consumables
            SpawnConsumable(consumable);
    }

    void RestorePlayerRelics()
    {
        foreach (int relic in GlobalControl.Instance.ownedRelics)
            SpawnRelic(relic);
    }

    private void DoReroll(int cost, bool unlock)
    {
        if (unlock)
        {
            isLocked = false;
            lockText.text = "Lock";
        }

        int numOfCards = 0;

        if (gold >= cost)
        {
            gold -= cost;

            foreach (Transform child in shop.transform)
            {
                numOfCards++;

                if (!isLocked)
                {
                    Destroy(child.gameObject);
                    numOfCards = 0;
                }
            }

            for (int i = 0; i < Mathf.Clamp(shopLevel + 3, 0, 7) - numOfCards; i++)
            {
                int randomNumber = Random.Range(1, 4);

                if (randomNumber != 3)
                    SpawnShopCard(availableShopCards[Random.Range(0, availableShopCards.Count)], false);
                else
                    SpawnShopCard(availableShopItems[Random.Range(0, availableShopItems.Count)], true);
            }
        }
    }

    // button
    public void RerollButton()
    {
        DoReroll(1, true);
    }

    // button
    public void UpgradeShop()
    {
        if (shopLevel < 6 && gold >= upgradeCost)
        {
            gold = gold - upgradeCost;
            shopLevel++;
            upgradeCost = nextUpgrade;
            nextUpgrade = Mathf.Clamp(nextUpgrade + 2, 0, 11);
            SetAvailableShopCards();
        }
    }

    // button
    public void LockShop()
    {
        if (isLocked == false)
            lockText.text = "Unlock";
        else
            lockText.text = "Lock";

        isLocked = !isLocked;
    }

    // button
    public void AdvanceButton()
    {
        tradingHubUI.SetActive(false);
        turnNumber++;
        FillChoices();
    }

    // button
    public void LoadBattleScene()
    {
        SaveData();
        SceneManager.LoadScene(1);
    }

    // button
    public void RestartButton()
    {
        Destroy(GlobalControl.Instance.gameObject);
        SceneManager.LoadScene(0);
    }

    // button
    public void DebugButton()
    {
        GlobalControl.Instance.debugMode = true;
        gold += 999;
        credits += 9999;
    }
}
