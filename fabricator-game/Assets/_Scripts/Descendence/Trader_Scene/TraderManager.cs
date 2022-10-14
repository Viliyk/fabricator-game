using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TraderManager : MonoBehaviour
{
    [SerializeField] private GameObject cardTemplate = null;
    [SerializeField] private GameObject consumableTemplate = null;
    [SerializeField] private GameObject relicTemplate = null;
    [SerializeField] private GameObject shop = null;
    [SerializeField] private GameObject board = null;
    [SerializeField] private GameObject hand = null;
    [SerializeField] private GameObject relics = null;
    [SerializeField] private TMP_Text turnText = null;
    [SerializeField] private TMP_Text livesText = null;
    [SerializeField] private TMP_Text creditsText = null;
    [SerializeField] private GameObject buyZone = null;
    [SerializeField] private GameObject shopHighlight = null;
    [SerializeField] private GameObject boardHighlight = null;

    private ThisCard createdCard;
    private GameObject spawnedCard;
    private ThisConsumable createdConsumable;
    private GameObject spawnedConsumable;
    private ThisRelic createdRelic;
    private GameObject spawnedRelic;

    public int turnNumber;
    public int lives;
    public int credits;
    //private int[] yourCards;
    private List<int> yourCards;
    private int[] handCards;

    private List<int> availableRelics = new List<int>();
    private List<int> availableConsumables = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        LoadData();
        RestorePlayerBoard();
        RestorePlayerHand();

        // spawn relics in the shop
        availableRelics.Clear();
        foreach (KeyValuePair<int, Relic> relic in RelicDB.relicList)
        {
            if (relic.Value.tier == 1)
                availableRelics.Add(relic.Value.id);
        }
        foreach (int x in GlobalControl.Instance.ownedRelics)
            availableRelics.Remove(x);

        for (int i = 0; i < 3; i++)
        {
            if (availableRelics.Count == 0)
                break;

            int x = availableRelics[Random.Range(0, availableRelics.Count)];
            SpawnShopRelic(x);
            availableRelics.Remove(x);
        }


        // spawn consumables in the shop
        availableConsumables.Clear();
        foreach (KeyValuePair<int, Consumable> consumable in ConsumableDB.consumableList)
        {
            if (consumable.Value.tier == 1)
                availableConsumables.Add(consumable.Value.id);
        }

        for (int i = 0; i < 3; i++)
        {
            int x = availableConsumables[Random.Range(0, availableConsumables.Count)];
            SpawnShopConsumable(x);
            availableConsumables.Remove(x);
        }
    }

    // Update is called once per frame
    void Update()
    {
        turnText.text = "Turn " + turnNumber;
        livesText.text = "Lives: " + lives;
        creditsText.text = "Credits: " + credits;

        ThisRelic[] relics = shop.transform.GetComponentsInChildren<ThisRelic>();
        foreach (ThisRelic relic in relics)
        {
            if (relic.clicked)
            {
                if (credits < 100)
                    relic.clicked = false;
                else
                {
                    relic.clicked = false;
                    credits -= 100;

                    relic.BuyThisRelic();
                }
            }
        }

        ThisConsumable[] consumables = shop.transform.GetComponentsInChildren<ThisConsumable>();
        foreach (ThisConsumable consumable in consumables)
        {
            if (consumable.clicked)
            {
                if (credits < 25)
                    consumable.clicked = false;
                else
                {
                    consumable.clicked = false;
                    credits -= 25;
                    SpawnConsumable(consumable.id);
                    consumable.BuyThisConsumable();
                }
            }
        }
    }

    public void SellMinion(ThisCard card)
    {
        credits += card.attack + card.health;
        Destroy(card.gameObject, 0.01f);
    }

    void SpawnShopRelic(int id)
    {
        createdRelic = relicTemplate.GetComponent<ThisRelic>();
        createdRelic.thisId = id;
        createdRelic.inShop = true;
        spawnedRelic = Instantiate(relicTemplate, new Vector3(0, 0, 0), Quaternion.identity);
        spawnedRelic.transform.SetParent(shop.transform, false);

        //spawnedCard.GetComponent<Draggable>().ChangeEnumToShop();
    }

    void SpawnShopConsumable(int id)
    {
        createdConsumable = consumableTemplate.GetComponent<ThisConsumable>();
        createdConsumable.thisId = id;
        createdConsumable.inShop = true;
        spawnedConsumable = Instantiate(consumableTemplate, new Vector3(0, 0, 0), Quaternion.identity);
        spawnedConsumable.transform.SetParent(shop.transform, false);
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
            //StartTripleCheck(id);
        }
        return createdCard;
    }

    public void SpawnConsumable(int id)
    {
        if (hand.transform.childCount < 10)
        {
            createdConsumable = consumableTemplate.GetComponent<ThisConsumable>();
            createdConsumable.thisId = id;
            spawnedConsumable = Instantiate(consumableTemplate, new Vector3(0, 0, 0), Quaternion.identity);
            spawnedConsumable.transform.SetParent(hand.transform, false);
            //createdConsumable = spawnedConsumable.GetComponent<ThisConsumable>();
        }
    }

    void SaveData()
    {
        GlobalControl.Instance.EmptyContainers();       // empty containers

        foreach (Transform a in board.transform)        // save stats of your board cards
        {
            ThisCard card = a.GetComponent<ThisCard>();
            if (card != null)
            {
                GlobalControl.Instance.newYourCards.Add(Container.SetValues
                    (GlobalControl.Instance.gameObject, card.cost, card.attack, card.health, card.golden, card.admission, card.shield,
                    card.cleave, card.guard, card.vengeance, card.command));
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
    }

    void LoadData()
    {
        yourCards = GlobalControl.Instance.yourCards;
        handCards = GlobalControl.Instance.handCards;

        GlobalControl.Instance.buyZone = buyZone;
        GlobalControl.Instance.shopHighlight = shopHighlight;
        GlobalControl.Instance.boardHighlight = boardHighlight;

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

                if (GlobalControl.Instance.newYourCards[i] != null)     // modify their stats to match what they were
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

    // button
    public void ContinueButton()
    {
        SaveData();
        SceneManager.LoadScene(0);
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
        //gold += 999;
        credits += 9999;
    }
}
