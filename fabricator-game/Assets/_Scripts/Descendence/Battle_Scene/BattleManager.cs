using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Fabricator.Units;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject unitTemplate = null;
    [SerializeField] private UnitSpawner unitSpawner = null;
    [SerializeField] private EnemySpawner enemySpawner = null;

    [SerializeField] private GameObject cardTemplate = null;
    [SerializeField] private GameObject itemTemplate = null;
    [SerializeField] private GameObject consumableTemplate = null;
    [SerializeField] private GameObject backline = null;
    [SerializeField] private GameObject hand = null;
    [SerializeField] private GameObject enemyHand = null;
    [SerializeField] private GameObject useZone = null;
    [SerializeField] private GameObject backlineHighlight = null;
    [SerializeField] private TMP_Text livesText = null;
    [SerializeField] private TMP_Text turnText = null;
    [SerializeField] private TMP_Text speedUpText = null;
    [SerializeField] private TMP_Text timeLeftText = null;
    [SerializeField] private TMP_Text energyText = null;
    [SerializeField] private TMP_Text enemyEnergyText = null;
    [SerializeField] private TMP_Text enemyAttackText = null;
    [SerializeField] private TMP_Text baseUpText = null;
    [SerializeField] private GameObject startButton = null;
    [SerializeField] private GameObject speedUpButton = null;
    [SerializeField] private GameObject bullet = null;

    [SerializeField] private GameObject victoryText;
    [SerializeField] private GameObject defeatText;

    public static float battleSpeed;
    public static float chargeRate;

    private List<int> yourCards;
    private Draggable draggable;
    private ThisCard createdCard;
    private GameObject spawnedCard;
    private ThisItem createdItem;
    private GameObject spawnedItem;
    private ThisConsumable createdConsumable;
    private GameObject spawnedConsumable;
    private ThisCard attacker;
    private ThisCard enemyAttacker;
    private int lives;
    private int turnNumber;
    private bool enemyTurn;
    private bool buttonToggle;
    private bool endBattle;
    private bool pauseBattle = true;
    private bool checkForClick = false;
    private int targetedConsumable;

    private List<int> availableEnemies = new List<int>();
    private List<ThisCard> frontlineMinions = new List<ThisCard>();
    private List<ThisCard> backlineMinions = new List<ThisCard>();
    private List<ThisCard> yourHand = new List<ThisCard>();
    private List<ThisItem> yourItems = new List<ThisItem>();
    private List<ThisCard> yourStasisMinions = new List<ThisCard>();
    private List<ThisCard> enemyFrontlineMinions = new List<ThisCard>();
    private List<ThisCard> enemyBacklineMinions = new List<ThisCard>();
    private List<ThisCard> enemyStasisMinions = new List<ThisCard>();

    public CanTakeDamage yourCommander;
    public CanTakeDamage enemyCommander;

    public ThisCard inactiveMinion;
    public GameObject inactiveMinionBlueprint;

    private double timeLeft = 10;
    public float energy;
    private float energyRate = 10.0f;
    public float enemyEnergy;
    private float enemyEnergyRate = 4.0f;
    private double timeToEnemyAttack = 15;

    private GameObject spawnedBullet;

    public int frontlineSlots = 10;
    public int backlineSlots = 6;
    public int enemyBacklineSlots = 6;

    private int baseUpCost = 50;

    // targeted admission variables
    private ThisCard targetedAdmissionCard = null;
    private bool sentinelWasTargeted = false;
    private bool chimeraWasTargeted = false;
    private bool defectWasTargeted = false;
    private bool compoundWasTargeted = false;
    private bool animaWasTargeted = false;

    //public AudioSource audioSource;
    //private IEnumerator coroutineAudioFadeIn;

    void Start()
    {
        GlobalControl.Instance.battleManager = this;
        GlobalControl.Instance.useZone = useZone;
        GlobalControl.Instance.backlineHighlight = backlineHighlight;
        GlobalControl.Instance.targetMode = false;

        yourCards = GlobalControl.Instance.yourCards;   // load player's cards from GlobalControl
        turnNumber = GlobalControl.Instance.turnNumber;
        lives = GlobalControl.Instance.lives;
        battleSpeed = 0.02f;
        buttonToggle = false;
        speedUpText.text = "Speed Up";

        chargeRate = battleSpeed;

        energy = 100;
        enemyEnergy = 50;

        SetAvailableEnemies();      // enemies can be one tier higher every 3 turns
        RestorePlayerBoard();       // spawn player's cards on board
        RestorePlayerHand();        // spawn player's hand back

        //audioSource.time = 13.2f;
        //coroutineAudioFadeIn = AudioFadeIn(audioSource);
        //StartCoroutine(coroutineAudioFadeIn);

        SpawnEnemyCards();

        enemyCommander.HP = turnNumber * 10;

        yourCommander.HP = lives;
    }

    // Text elements, loading next scene
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && GlobalControl.Instance.targetMode)
        {
            if (inactiveMinion != null)
            {
                GlobalControl.Instance.targetMode = false;
                Destroy(inactiveMinion.gameObject);
            }
        }


        lives = (int)yourCommander.HP;


        livesText.text = "Lives: " + lives;
        turnText.text = "Turn " + turnNumber;
        timeLeftText.text = timeLeft.ToString("F1");
        energyText.text = energy.ToString("F0");
        enemyEnergyText.text = enemyEnergy.ToString("F0");
        baseUpText.text = "Upgrade: " + baseUpCost;
        enemyAttackText.text = timeToEnemyAttack.ToString("F1");

        // Load shop scene
        if (endBattle == true)
        {
            if (GlobalControl.Instance.currentNode == 4)    // load trader scene instead
                SceneManager.LoadScene(2);
            else
                SceneManager.LoadScene(0);
        }
    }

    IEnumerator BattleSequence()
    {
        while (pauseBattle != true)
        {
            yield return new WaitForSeconds(battleSpeed);

            // tick down time
            timeLeft = System.Math.Round(timeLeft - chargeRate, 3);

            // gain energy
            energy += energyRate * chargeRate * 3;
            enemyEnergy += enemyEnergyRate * chargeRate * 10;

            // tick down enemy attack
            timeToEnemyAttack = System.Math.Round(timeToEnemyAttack - chargeRate, 3);
            if (timeToEnemyAttack <= 0)
            {
                timeToEnemyAttack = 15;

                //foreach (ThisCard card in enemyBacklineMinions)
                //{
                //    card.backline = false;
                //    enemyFrontlineMinions.Add(card);
                //    card.transform.SetParent(enemyFrontline.transform);
                //}
                //enemyBacklineMinions.RemoveAll(item => !item.backline);
            }

            // tick down build cooldowns
            for (int i = 0; i < yourHand.Count; i++)
            {
                ThisCard t = yourHand[i];

                if (t.bought && !t.onHold)
                {
                    t.currentEnergy += chargeRate * 15;
                    PayEnergy(chargeRate * 15, false);

                    if (t.currentEnergy >= t.energyCost)
                    {
                        TriggerOnBuild(t, false);
                        t.currentEnergy = 0;
                    }
                }
            }

            // tick down enemy build cooldowns
            for (int i = 0; i < enemyStasisMinions.Count; i++)
            {
                ThisCard t = enemyStasisMinions[i];

                t.currentEnergy += chargeRate * 15;
                PayEnergy(chargeRate * 15, true);

                if (t.currentEnergy >= t.energyCost)
                {
                    TriggerOnBuild(t, true);
                    t.currentEnergy = 0;
                }
            }

            // tick down item cooldowns
            for (int i = 0; i < yourItems.Count; i++)
            {
                ThisItem item = yourItems[i];
                Draggable d = item.GetComponentInParent<Draggable>();

                if (d.typeOfCard == Draggable.Slot.BUILDING)
                    item.timeCost -= chargeRate;

                if (item.timeCost <= 0)
                {
                    d.typeOfCard = Draggable.Slot.BATTLE;
                    item.DeactivateStasisSlider();
                    item.timeCost = item.baseTimeCost;
                }
            }

            // battle won
            if (enemyCommander.HP <= 0)
            {
                pauseBattle = true;
                victoryText.SetActive(true);
                yield return new WaitForSeconds(battleSpeed * 120);
                BattleWon();
            }

            // battle lost
            if (yourCommander.HP <= 0)
            {
                pauseBattle = true;
                defeatText.SetActive(true);
                yield return new WaitForSeconds(battleSpeed * 120);
                BattleLost();
            }
        }
    }

    void TriggerOnBuild(ThisCard activatedCard, bool isEnemy)
    {
        ThisCard spawnedCard = null;

        List<ThisCard> allyHand = new List<ThisCard>();
        List<ThisCard> enemyHand = new List<ThisCard>();
        List<ThisCard> allyBackline = new List<ThisCard>();
        List<ThisCard> enemyBackline = new List<ThisCard>();

        if (isEnemy == false)
        {
            if (unitSpawner == null)
                return;

            if (!activatedCard.station)
                unitSpawner.SpawnUnit(activatedCard, false);

            allyHand = yourHand;
            allyBackline = backlineMinions;
            enemyHand = enemyStasisMinions;
            enemyBackline = enemyBacklineMinions;
        }
        else if (isEnemy == true)
        {
            if (enemySpawner == null)
                return;

            if (!activatedCard.station)
                enemySpawner.SpawnUnit(activatedCard, true);

            allyHand = enemyStasisMinions;
            allyBackline = enemyBacklineMinions;
            enemyHand = yourHand;
            enemyBackline = backlineMinions;
        }

        // on build abilities
        for (int i = 0; i < activatedCard.abilityList.Count; i++)
        {
            switch (activatedCard.abilityList[i][0])
            {
                default:
                    break;

                case 10:    // alchemist: give your other blueprints +1 health
                    {
                        foreach (ThisCard card in allyHand)
                        {
                            if (card != activatedCard)
                                card.health += activatedCard.abilityList[i][1];
                        }
                    }
                    break;
                case 11:    // mobile barrier: give your other backline minions barrier
                    {
                        foreach (ThisCard card in allyBackline)
                        {
                            if (card != spawnedCard)
                                card.abilityList.Add(CardDB.abilityDB[1]);
                        }
                    }
                    break;
                case 12:    // fungus farms: give your other chimera blueprints +1 health
                    {
                        foreach (ThisCard card in allyHand)
                        {
                            if (card != activatedCard && card.type == "Chimera")
                            {
                                card.attack += activatedCard.abilityList[i][1];
                                card.health += activatedCard.abilityList[i][1];
                            }
                        }
                    }
                    break;
                case 15:    // generator
                    {
                        energy += activatedCard.abilityList[i][1];
                    }
                    break;
                case 16:
                    {
                        foreach (ThisCard card in allyHand)
                        {
                            if (card.transform.GetSiblingIndex() == activatedCard.transform.GetSiblingIndex() + 1
                                || card.transform.GetSiblingIndex() == activatedCard.transform.GetSiblingIndex() - 1)
                            {
                                card.currentEnergy += activatedCard.abilityList[i][1];
                            }
                        }
                    }
                    break;
            }
        }
    }

    public void SpawnUnit(ThisCard activatedCard, bool isEnemy)
    {
        // ******************* TEST *****************
        //Vector3 spawnPoint;
        //GameObject spawnedUnit;
        //TestUnit unitStats;

        //if (!isEnemy)
        //    spawnPoint = new Vector3(0, 0, -5);
        //else
        //    spawnPoint = new Vector3(0, 0, 12);

        //spawnedUnit = Instantiate(unitTemplate, spawnPoint, Quaternion.identity);
        //unitStats = spawnedUnit.GetComponent<TestUnit>();
        //unitStats.AD = activatedCard.attack;
        //unitStats.HP = activatedCard.health;

        //if (isEnemy)
        //{
        //    unitStats.isEnemy = true;
        //    spawnedUnit.layer = 7;
        //}
        // ******************************************


        //if (t.admission)
        //{
        //    inactiveMinion = t;
        //    inactiveMinionBlueprint = playedCard;
        //    GlobalControl.Instance.targetMode = true;
        //    SetViableTargets(t);
        //}
        //else
        //ActivateMinion(playedCard, t, null);
    }

    void SetAvailableEnemies()
    {
        int level = 1;
        int a = 0;
        for (int i = 0; i < turnNumber; i++)
        {
            a++;
            if (a == 3)
            {
                level++;
                a = 0;
            }
        }

        availableEnemies.Clear();
        foreach (KeyValuePair<int, Card> card in CardDB.cardList)
        {
            if (card.Value.tier <= level && card.Value.tier > 0)
                availableEnemies.Add(card.Value.id);
        }
    }

    void SpawnEnemyCards()
    {
        for (int i = 0; i < Mathf.Clamp(turnNumber, 1, 7); i++)     // spawn enemy's cards on enemy hand
        {
            if (enemyHand.transform.childCount < 7)
            {
                int buffStartsAfter = 5;
                int enemyBuff = turnNumber - buffStartsAfter;

                createdCard = cardTemplate.GetComponent<ThisCard>();
                createdCard.thisId = availableEnemies[Random.Range(0, availableEnemies.Count)];     // decide what cards the enemy will get
                spawnedCard = Instantiate(cardTemplate, new Vector3(0, 0, 0), Quaternion.identity);
                spawnedCard.transform.localScale = new Vector3(0.50f, 0.50f, 1);
                spawnedCard.transform.SetParent(enemyHand.transform, false);
                createdCard = spawnedCard.GetComponent<ThisCard>();
                createdCard.isEnemy = true;
                draggable = spawnedCard.GetComponent<Draggable>();
                draggable.ChangeEnum(Draggable.Slot.BATTLEHAND);
                draggable.enabled = false;

                enemyStasisMinions.Add(createdCard);
                //createdCard.ActivateStasisSlider();
            }
        }
    }

    void RestorePlayerBoard()
    {
        for (int i = 0; i < yourCards.Count; i++)
        {
            //if (yourCards.Count != GlobalControl.Instance.newYourCards.Count)
            //    break;

            if (hand.transform.childCount < 10)
            {
                int unit = yourCards[i];

                if (unit != 0)
                {
                    createdCard = cardTemplate.GetComponent<ThisCard>();    // spawn plain copies of player's cards
                    createdCard.thisId = unit;
                    spawnedCard = Instantiate(cardTemplate, new Vector3(0, 0, 0), Quaternion.identity);
                    spawnedCard.transform.SetParent(hand.transform, false);
                    spawnedCard.GetComponent<Draggable>().ChangeEnum(Draggable.Slot.BATTLEHAND);   // change enum to BATTLEHAND

                    createdCard = spawnedCard.GetComponent<ThisCard>();     // modify their stats to match what they were
                    //if (GlobalControl.Instance.newYourCards[i] != null)
                    //{
                    //    createdCard.cost = GlobalControl.Instance.newYourCards[i].cost;
                    //    createdCard.attack = GlobalControl.Instance.newYourCards[i].attack;
                    //    createdCard.health = GlobalControl.Instance.newYourCards[i].health;
                    //    createdCard.golden = GlobalControl.Instance.newYourCards[i].golden;
                    //    createdCard.admission = GlobalControl.Instance.newYourCards[i].admission;
                    //    createdCard.shield = GlobalControl.Instance.newYourCards[i].shield;
                    //    createdCard.cleave = GlobalControl.Instance.newYourCards[i].cleave;
                    //    createdCard.guard = GlobalControl.Instance.newYourCards[i].guard;
                    //    createdCard.vengeance = GlobalControl.Instance.newYourCards[i].vengeance;
                    //    createdCard.command = GlobalControl.Instance.newYourCards[i].command;
                    //}

                    createdCard.ActivateStasisSlider();
                    createdCard.ActivateTargetBorder();
                    yourHand.Add(createdCard);
                }
            }
        }

        // restore player items
        foreach (int item in GlobalControl.Instance.yourItems)
            SpawnPlayerItem(item);
    }

    void RestorePlayerHand()
    {
        foreach (int consumable in GlobalControl.Instance.battleConsumables)      // restore consumables
            SpawnConsumable(consumable);
    }

    public void ItemFunctionality(ThisItem playedItem, ThisCard target)
    {
        bool activated = false;
        Draggable d = playedItem.GetComponent<Draggable>();

        if (energy >= playedItem.energyCost && d.typeOfCard != Draggable.Slot.BUILDING)
        {
            switch (playedItem.id)
            {
                default:
                    print("Error: Invalid item id");
                    break;
            }

            if (activated)
            {
                PayEnergy(playedItem.energyCost, false);
                BuildCooldown(playedItem.gameObject);
            }
        }
    }

    public void ConsumableFunctionality(ThisConsumable consumable, ThisCard target)
    {
        switch (consumable.id)
        {
            default:
                print("Error: Invalid consumable id");
                break;

            case 2:     // wide buff
                {
                    foreach (ThisCard card in frontlineMinions)
                    {
                        card.attack++;
                        card.health++;
                    }
                }
                break;
            case 3:     // guard potion
                {
                    targetedConsumable = consumable.id;

                    List<ThisCard> allMinions = new List<ThisCard>();
                    allMinions.AddRange(frontlineMinions);
                    allMinions.AddRange(enemyBacklineMinions);

                    foreach (ThisCard card in allMinions)
                    {
                        if (card.thisIsTargeted == true)
                        {
                            foreach (ThisCard a in allMinions)
                                a.DeactivateTargetBorder();

                            card.guard = true;
                            card.thisIsTargeted = false;
                            checkForClick = false;
                            return;
                        }
                    }
                    foreach (ThisCard card in allMinions)
                        card.ActivateTargetBorder();
                    checkForClick = true;
                }
                break;
            case 5:     // defected banana
                {
                    targetedConsumable = consumable.id;

                    List<ThisCard> cards = new List<ThisCard>();
                    cards.AddRange(frontlineMinions);
                    cards.AddRange(backlineMinions);

                    foreach (ThisCard card in cards)
                    {
                        if (card.thisIsTargeted == true)
                        {
                            foreach (ThisCard a in cards)
                                a.DeactivateTargetBorder();

                            card.attack += 3;
                            card.health += 3;
                            card.thisIsTargeted = false;
                            checkForClick = false;
                            return;
                        }
                    }
                    foreach (ThisCard card in cards)
                        card.ActivateTargetBorder();
                    checkForClick = true;
                }
                break;
            case 7:     // borger
                {
                    if (target.type == "Chimera" && !target.isEnemy)
                    {
                        target.attack++;
                        target.health++;

                        Destroy(consumable.gameObject);
                    }
                }
                break;
        }
    }

    public void SpawnConsumable(int id)
    {
        if (hand.transform.childCount < 10)
        {
            createdConsumable = consumableTemplate.GetComponent<ThisConsumable>();
            createdConsumable.thisId = id;
            spawnedConsumable = Instantiate(consumableTemplate, new Vector3(0, 0, 0), Quaternion.identity);
            spawnedConsumable.transform.SetParent(hand.transform, false);
            spawnedConsumable.GetComponent<Draggable>().ChangeEnum(Draggable.Slot.BATTLE);
        }
    }

    public ThisItem SpawnPlayerItem(int id)
    {
        if ((hand.transform.childCount < 10))
        {
            createdItem = itemTemplate.GetComponent<ThisItem>();
            createdItem.thisId = id;
            spawnedItem = Instantiate(itemTemplate, new Vector3(0, 0, 0), Quaternion.identity);
            createdItem = spawnedItem.GetComponent<ThisItem>();
            yourItems.Add(createdItem);

            spawnedItem.transform.SetParent(hand.transform, false);
            //spawnedItem.GetComponent<Draggable>().ChangeEnumToBattleHand();
            spawnedItem.GetComponent<Draggable>().ChangeEnum(Draggable.Slot.BATTLE);

        }
        return createdItem;
    }

    public void ActivateMinion(GameObject playedCard, ThisCard t, ThisCard target)
    {
        if (playedCard == null)
            playedCard = inactiveMinionBlueprint;
        if (t == null)
            t = inactiveMinion;

        BuildCooldown(playedCard);
        backlineMinions.Add(t);
        t.backline = true;
        t.onField = true;
        TriggerOnPlay(t, target);
    }

    void BuildCooldown(GameObject card)
    {
        Draggable d = card.GetComponent<Draggable>();
        ThisCard t = card.GetComponent<ThisCard>();
        ThisItem i = card.GetComponent<ThisItem>();
        //d.EndDrag();
        d.ChangeEnum(Draggable.Slot.BUILDING);
        //d.typeOfCard = Draggable.Slot.BUILDING;

        //if (t != null)
        //    t.ActivateStasisSlider();
        if (i != null)
            i.ActivateStasisSlider();
    }

    /*IEnumerator AudioFadeIn(AudioSource audioSource)
    {
        audioSource.volume = 0;

        while (audioSource.volume < 0.3f)
        {
            audioSource.volume += 0.01f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator AudioFadeOut(AudioSource audioSource)
    {
        StopCoroutine(coroutineAudioFadeIn);
        while (audioSource.volume > 0)
        {
            audioSource.volume -= 0.003f;
            yield return new WaitForSeconds(battleSpeed / 100);
        }
    }*/

    void SetViableTargets(ThisCard playedCard)
    {
        List<ThisCard> allies = new List<ThisCard>();
        allies.AddRange(frontlineMinions);
        allies.AddRange(backlineMinions);
        List<ThisCard> enemies = new List<ThisCard>();
        enemies.AddRange(enemyFrontlineMinions);
        enemies.AddRange(enemyBacklineMinions);

        bool targetFound = false;

        if (playedCard == null)
            playedCard = inactiveMinion;

        switch (playedCard.id)
        {
            default:
                break;

            case 2:
                {
                    if (enemies.Count > 0)
                    {
                        foreach (ThisCard card in enemies)
                        {
                            card.viableTarget = true;
                            targetFound = true;
                        }
                    }
                }
                break;
            case 3:     // sentinel scout
                {
                    if (allies.Count > 0)
                    {
                        // allied sentinels
                        foreach (ThisCard card in allies)
                        {
                            if (card.type == "Sentinel")
                            {
                                card.viableTarget = true;
                                targetFound = true;
                            }
                        }
                    }
                }
                break;
        }

        if (!targetFound)
        {
            GlobalControl.Instance.targetMode = false;
            ActivateMinion(null, null, null);
        }
    }

    public void AdmissionFunctionality(ThisCard playedCard, ThisCard target)
    {
        List<ThisCard> allies = new List<ThisCard>();
        allies.AddRange(frontlineMinions);
        allies.AddRange(backlineMinions);
        List<ThisCard> enemies = new List<ThisCard>();
        enemies.AddRange(enemyFrontlineMinions);
        enemies.AddRange(enemyBacklineMinions);

        int goldenMultiplier = 1;           // if the played card is golden this automatically scales effects up 2*
        if (playedCard.golden == true)
            goldenMultiplier = 2;

        switch (playedCard.id)
        {
            default:
                print("Error: This card doesn't have admission");
                break;

            case 6:     // temp chimera #1
                {
                    foreach (ThisCard card in allies)
                    {
                        if (card.type == "Chimera" && card != playedCard)
                            card.attack += 2 * goldenMultiplier;
                    }
                }
                break;
            case 18:    // horrendous growth
                {
                    foreach (ThisCard card in allies)
                    {
                        if (card != playedCard)
                        {
                            playedCard.attack += card.attack * goldenMultiplier;
                            playedCard.health += card.health * goldenMultiplier;
                            Destroy(card.gameObject);
                        }
                    }
                }
                break;
            case 5:     // junkbot
                {
                    foreach (ThisCard card in allies)
                    {
                        if (card.type == "Defect" && card != playedCard)
                            card.health += 1 * goldenMultiplier;
                    }
                }
                break;
            case 12:    // temp defect #4
                {
                    foreach (ThisCard card in allies)
                    {
                        if (card.type == "Defect" && card != playedCard)
                        {
                            card.attack += 4 * goldenMultiplier;
                        }
                    }
                }
                break;
            case 3:     // sentinel scout
                {
                    if (target != null)
                    {
                        target.attack += 2;
                        target.health += 2;
                    }
                }
                break;
            case 23:    // old spare-part vendor, now joe is id 23
                {
                    bool sentinelWasFound = false;
                    bool chimeraWasFound = false;
                    bool defectWasFound = false;

                    targetedAdmissionCard = playedCard;

                    foreach (ThisCard card in allies)
                    {
                        if (card.thisIsTargeted == true)
                        {
                            card.attack += 1 * goldenMultiplier;
                            card.health += 1 * goldenMultiplier;
                            card.thisIsTargeted = false;
                            if (card.type == "Sentinel")
                                sentinelWasTargeted = true;
                            if (card.type == "Chimera")
                                chimeraWasTargeted = true;
                            if (card.type == "Defect")
                                defectWasTargeted = true;
                        }
                    }
                    foreach (ThisCard card in allies)
                    {
                        if (card.type == "Sentinel" && card != playedCard)
                        {
                            if (sentinelWasTargeted == false)
                            {
                                card.ActivateTargetBorder();
                                sentinelWasFound = true;
                                checkForClick = true;
                            }
                            else
                                card.DeactivateTargetBorder();
                        }
                        if (card.type == "Chimera" && card != playedCard)
                        {
                            if (chimeraWasTargeted == false)
                            {
                                card.ActivateTargetBorder();
                                chimeraWasFound = true;
                                checkForClick = true;
                            }
                            else
                                card.DeactivateTargetBorder();
                        }
                        if (card.type == "Defect" && card != playedCard)
                        {
                            if (defectWasTargeted == false)
                            {
                                card.ActivateTargetBorder();
                                defectWasFound = true;
                                checkForClick = true;
                            }
                            else
                                card.DeactivateTargetBorder();
                        }
                    }

                    if (sentinelWasFound == false)
                        sentinelWasTargeted = true;
                    if (chimeraWasFound == false)
                        chimeraWasTargeted = true;
                    if (defectWasFound == false)
                        defectWasTargeted = true;

                    if (sentinelWasTargeted == true)
                        sentinelWasFound = false;
                    if (chimeraWasTargeted == true)
                        chimeraWasFound = false;
                    if (defectWasTargeted == true)
                        defectWasFound = false;

                    if (sentinelWasFound == false && chimeraWasFound == false && defectWasFound == false)
                    {
                        sentinelWasTargeted = false;
                        chimeraWasTargeted = false;
                        defectWasTargeted = false;
                        checkForClick = false;
                    }
                }
                break;
            case 25:    // cockroach
                {
                    for (int i = 0; i < goldenMultiplier; i++)
                        SpawnConsumable(7);
                }
                break;
            case 27:    // alchemist
                {
                    foreach (ThisCard card in yourHand)
                    {
                        if (!card.structure)
                            card.attack += 1;
                    }
                }
                break;
        }
    }

    void TriggerOnPlay(ThisCard playedCard, ThisCard target)
    {
        if (playedCard.admission)
            AdmissionFunctionality(playedCard, target);
    }

    public void PayEnergy(float amount, bool isEnemy)
    {
        if (!isEnemy)
            energy -= amount;

        if (isEnemy)
            enemyEnergy -= amount;
    }

    List<int> CheckForAbility(ThisCard card, int abilityId)
    {
        for (int i = 0; i < card.abilityList.Count; i++)
        {
            if (card.abilityList[i][0] == abilityId)
                return card.abilityList[i];
        }

        return null;
    }

    void RemoveAbility(ThisCard card, int abilityId)
    {
        for (int i = 0; i < card.abilityList.Count; i++)
        {
            if (card.abilityList[i][0] == abilityId)
            {
                card.abilityList.RemoveAt(i);
                i--;
            }
        }
    }

    void BattleWon()
    {
        GlobalControl.Instance.turnNumber++;
        GlobalControl.Instance.lives = lives;
        endBattle = true;
    }

    void BattleLost()
    {
        RestartButton();
    }

    // button
    public void StartBattle()
    {
        timeLeft = 1000;
        pauseBattle = false;
        StartCoroutine(BattleSequence());    // start combat

        speedUpButton.SetActive(true);      // change start button to speed-up button
        startButton.SetActive(false);
    }

    // button
    public void UpgradeButton()
    {
        if (energy >= baseUpCost)
        {
            PayEnergy(baseUpCost, false);
            baseUpCost += 100;

            energyRate += 5f;

            //RectTransform rt = backline.GetComponent<RectTransform>();
            //RectTransform highlightRT = backlineHighlight.GetComponent<RectTransform>();

            //backlineSlots += 2;
            //rt.sizeDelta += new Vector2(120, 0);
            //highlightRT.sizeDelta += new Vector2(120, 0);
        }
    }

    // button
    public void SpeedUpBattle()
    {
        if (buttonToggle == false)
        {
            chargeRate *= 5;
            speedUpText.text = "Speed Down";
        }
        else if (buttonToggle == true)
        {
            chargeRate /= 5;
            speedUpText.text = "Speed Up";
        }
        buttonToggle = !buttonToggle;
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
        energy += 9999;
    }
}
