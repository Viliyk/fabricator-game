using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private GameObject unitTemplate = null;


    [SerializeField] private GameObject cardTemplate = null;
    [SerializeField] private GameObject itemTemplate = null;
    [SerializeField] private GameObject consumableTemplate = null;
    [SerializeField] private GameObject frontline = null;
    [SerializeField] private GameObject backline = null;
    [SerializeField] private GameObject enemyFrontline = null;
    [SerializeField] private GameObject enemyBackline = null;
    [SerializeField] private GameObject hand = null;
    [SerializeField] private GameObject enemyHand = null;
    [SerializeField] private GameObject useZone = null;
    [SerializeField] private GameObject frontlineHighlight = null;
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

    //private int[] yourCards;
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
    private ThisCard survivingEnemy;
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
    private List<ThisCard> tauntMinions = new List<ThisCard>();
    private List<ThisCard> enemyTauntMinions = new List<ThisCard>();

    private List<ThisCard> yourVengeances = new List<ThisCard>();
    private List<ThisCard> enemyVengeances = new List<ThisCard>();


    public ThisCard yourCommander;
    public ThisCard enemyCommander;

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

    private int buildIndex = 0;
    private bool reset = true;
    private int enemyBuildIndex = 0;
    private bool enemyReset = true;

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
        GlobalControl.Instance.frontlineHighlight = frontlineHighlight;
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

        SpawnEnemies();


        //enemyEnergyRate += turnNumber * 0.1f;

        enemyCommander.health = turnNumber * 10;

        yourCommander.health = lives;
    }

    // text elements, loading next scene
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






        //// enemy spawner
        //ThisCard createdEnemy = null;
        //ThisCard spawnedEnemy;
        //if (enemyStasisMinions.Count > 0)
        //    createdEnemy = enemyStasisMinions[Random.Range(0, enemyStasisMinions.Count)];

        //if (createdEnemy != null && enemyEnergy >= createdEnemy.energyCost && !pauseBattle && createdEnemy.GetComponent<Draggable>().typeOfCard == Draggable.Slot.BATTLEHAND && enemyBacklineMinions.Count < frontlineSlots)
        //{
        //    createdEnemy.copy = true;

        //    spawnedEnemy = Instantiate(createdEnemy);
        //    spawnedEnemy.transform.SetParent(enemyBackline.transform);
        //    spawnedEnemy.transform.localScale = new Vector3(0.5f, 0.5f, 0);
        //    enemyBacklineMinions.Add(spawnedEnemy);
        //    spawnedEnemy.onField = true;
        //    spawnedEnemy.backline = true;

        //    PayEnergy(createdEnemy.energyCost, true);
        //    BuildCooldown(createdEnemy.gameObject);

        //    if (GlobalControl.Instance.targetMode)
        //        SetViableTargets(inactiveMinion);
        //}




        lives = yourCommander.health;



        livesText.text = "Lives: " + lives;
        turnText.text = "Turn " + turnNumber;
        timeLeftText.text = timeLeft.ToString("F1");
        energyText.text = energy.ToString("F0");
        enemyEnergyText.text = enemyEnergy.ToString("F0");
        baseUpText.text = "Upgrade: " + baseUpCost;
        enemyAttackText.text = timeToEnemyAttack.ToString("F1");

        // load shop scene
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
        double burnCounter = 1;
        double rapidFireCounter = 0;

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

                foreach (ThisCard card in enemyBacklineMinions)
                {
                    card.backline = false;
                    enemyFrontlineMinions.Add(card);
                    card.transform.SetParent(enemyFrontline.transform);

                    if (CheckForAbility(card, 2) != null)
                        enemyTauntMinions.Add(card);
                }
                enemyBacklineMinions.RemoveAll(item => !item.backline);
            }

            // tick down burn
            if (burnCounter <= 0)
                burnCounter = 1;
            burnCounter = System.Math.Round(burnCounter - chargeRate, 3);

            // tick down rapid fire
            if (rapidFireCounter <= 0)
                rapidFireCounter = 0.2f;
            rapidFireCounter -= chargeRate;

            //// tick down enemy stasis minions
            //for (int i = 0; i < enemyStasisMinions.Count; i++)
            //{
            //    ThisCard t = enemyStasisMinions[i];
            //    Draggable d = t.GetComponentInParent<Draggable>();

            //    if (d.typeOfCard == Draggable.Slot.BUILDING)
            //        t.timeCost -= chargeRate;

            //    if (t.timeCost <= 0)
            //    {
            //        //enemyStasisMinions.Remove(card);
            //        //enemyMinions.Add(card);
            //        d.typeOfCard = Draggable.Slot.BATTLEHAND;
            //        t.DeactivateStasisSlider();
            //        t.timeCost = t.baseTimeCost;
            //    }
            //}

            // tick down your frontline
            for (int i = 0; i < frontlineMinions.Count; i++)
            {
                ThisCard card = frontlineMinions[i];

                if (card.cooldown > 0)
                    card.cooldown = System.Math.Round(card.cooldown - (chargeRate * (1 + card.speed)), 3);

                if (card.rapidFire && card.attack > 0 && rapidFireCounter <= 0)
                {
                    StartCoroutine(Attack(0, card, 1, null, false));
                }

                if (card.cooldown <= 0)
                {
                    card.cooldown = card.baseCooldown;
                    StartCoroutine(TriggerOnActivation(card, false));
                }

                if (card.burn > 0 && burnCounter <= 0)
                {
                    DealDamage(card, card.burn, card, false);
                    card.burn--;
                }
            }

            // tick down your backline
            for (int i = 0; i < backlineMinions.Count; i++)
            {
                bool fullSpeed = false;
                if (enemyFrontlineMinions.Count > 0)
                    fullSpeed = true;

                ThisCard card = backlineMinions[i];

                if (card.cooldown > 0)
                {
                    if (fullSpeed)
                        card.cooldown = System.Math.Round(card.cooldown - (chargeRate * (1 + card.speed)), 3);
                    else
                        card.cooldown = System.Math.Round(card.cooldown - (chargeRate * (1 + card.speed) * (card.cooldown / card.baseCooldown)), 3);
                }

                if (card.rapidFire && card.attack > 0 && rapidFireCounter <= 0)
                {
                    StartCoroutine(Attack(0, card, 1, null, false));
                }

                if (card.burn > 0 && burnCounter <= 0)
                {
                    DealDamage(card, card.burn, card, false);
                    card.burn--;
                }

                if (card.cooldown <= 0)
                {
                    card.cooldown = card.baseCooldown;
                    StartCoroutine(TriggerOnActivation(card, false));
                }
            }

            // tick down build cooldowns
            for (int i = 0; i < yourHand.Count; i++)
            {
                ThisCard t = yourHand[i];
                Draggable d = t.GetComponentInParent<Draggable>();

                //if (t.gameObject.transform.GetSiblingIndex() == buildIndex)
                //{
                if (t.bought && !t.onHold)
                {
                    t.currentEnergy += chargeRate * 15;
                    PayEnergy(chargeRate * 15, false);

                    if (t.currentEnergy >= t.energyCost)
                    {
                        //d.typeOfCard = Draggable.Slot.BATTLEHAND;
                        //t.DeactivateStasisSlider();
                        //buildIndex++;
                        //SpawnToBackline(t.gameObject, false);
                        TriggerOnBuild(t, false);
                        t.currentEnergy = 0;
                    }
                }
                //}

                //if (buildIndex >= yourHand.Count && reset)
                //{
                //    reset = false;
                //    StartCoroutine(ResetBuild());
                //}
            }

            // tick down enemy build cooldowns
            for (int i = 0; i < enemyStasisMinions.Count; i++)
            {
                ThisCard t = enemyStasisMinions[i];
                Draggable d = t.GetComponentInParent<Draggable>();

                //if (t.gameObject.transform.GetSiblingIndex() == enemyBuildIndex)
                //{
                t.currentEnergy += chargeRate * 15;
                PayEnergy(chargeRate * 15, true);

                if (t.currentEnergy >= t.energyCost)
                {
                    //enemyBuildIndex++;
                    //SpawnToBackline(t.gameObject, true);
                    TriggerOnBuild(t, true);
                    t.currentEnergy = 0;
                }
                //}

                //if (enemyBuildIndex >= enemyStasisMinions.Count && enemyReset)
                //{
                //    enemyReset = false;
                //    StartCoroutine(ResetBuildEnemy());
                //}
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

            // tick down enemy frontline
            for (int i = 0; i < enemyFrontlineMinions.Count; i++)
            {
                ThisCard card = enemyFrontlineMinions[i];

                if (card.cooldown > 0)
                    card.cooldown = System.Math.Round(card.cooldown - chargeRate, 3);

                if (card.rapidFire && card.attack > 0 && rapidFireCounter <= 0)
                {
                    StartCoroutine(Attack(0, card, 1, null, true));
                }

                if (card.burn > 0 && burnCounter <= 0)
                {
                    DealDamage(card, card.burn, card, true);
                    card.burn--;
                }

                if (card.cooldown <= 0)
                {
                    card.cooldown = card.baseCooldown;
                    StartCoroutine(TriggerOnActivation(card, true));
                }
            }

            // tick down enemy backline
            for (int i = 0; i < enemyBacklineMinions.Count; i++)
            {
                bool fullSpeed = false;
                if (frontlineMinions.Count > 0)
                    fullSpeed = true;

                ThisCard card = enemyBacklineMinions[i];

                if (card.cooldown > 0)
                {
                    if (fullSpeed)
                        card.cooldown = System.Math.Round(card.cooldown - (chargeRate * (1 + card.speed)), 3);
                    else
                        card.cooldown = System.Math.Round(card.cooldown - (chargeRate * (1 + card.speed) * (card.cooldown / card.baseCooldown)), 3);
                }

                if (card.rapidFire && card.attack > 0 && rapidFireCounter <= 0)
                {
                    StartCoroutine(Attack(0, card, 1, null, true));
                }

                if (card.burn > 0 && burnCounter <= 0)
                {
                    DealDamage(card, card.burn, card, true);
                    card.burn--;
                }

                if (card.cooldown <= 0)
                {
                    card.cooldown = card.baseCooldown;
                    StartCoroutine(TriggerOnActivation(card, true));
                }
            }

            CheckForDeadMinions();

            // trigger vengeances
            bool vengeanceToggle = false;
            while (yourVengeances.Count != 0 || enemyVengeances.Count != 0)
            {
                VengeanceFunctionality(vengeanceToggle);
                vengeanceToggle = !vengeanceToggle;
            }

            // battle won
            if (enemyCommander.health <= 0)
            {
                pauseBattle = true;

                victoryText.SetActive(true);

                yield return new WaitForSeconds(battleSpeed * 70);
                BattleWon();
            }

            // battle lost
            if (yourCommander.health <= 0)
            {
                pauseBattle = true;

                defeatText.SetActive(true);

                int totalDamage = 0;

                foreach (Transform enemy in enemyBackline.transform)
                {
                    survivingEnemy = enemy.GetComponent<ThisCard>();
                    survivingEnemy.tierText.color = new Color(1, 0.92f, 0.016f, 1);
                    totalDamage += survivingEnemy.tier;
                }

                yield return new WaitForSeconds(battleSpeed * 70);
                BattleLost(totalDamage);
            }
        }
    }

    IEnumerator ResetBuild()
    {
        yield return new WaitForSeconds(0.08f / chargeRate);

        for (int i = 0; i < yourHand.Count; i++)
        {
            ThisCard t = yourHand[i];
            t.currentEnergy = 0;
            t.ActivateStasisSlider();
        }

        buildIndex = 0;
        reset = true;
    }

    IEnumerator ResetBuildEnemy()
    {
        yield return new WaitForSeconds(0.08f / chargeRate);

        for (int i = 0; i < enemyStasisMinions.Count; i++)
        {
            ThisCard t = enemyStasisMinions[i];
            t.currentEnergy = 0;
            t.ActivateStasisSlider();
        }

        enemyBuildIndex = 0;
        enemyReset = true;
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

    void SpawnEnemies()
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
        //SpawnPlayerCard(33, true);  // add a generator to hand
        //SpawnPlayerCard(35, true);  // add a turret to hand

        //int i = 0;
        //foreach (int unit in yourCards)
        //{
        //    if (hand.transform.childCount < 10)
        //    {
        //        if (unit != 0)
        //        {
        //            createdCard = cardTemplate.GetComponent<ThisCard>();    // spawn plain copies of player's cards
        //            createdCard.thisId = unit;
        //            spawnedCard = Instantiate(cardTemplate, new Vector3(0, 0, 0), Quaternion.identity);
        //            spawnedCard.transform.SetParent(hand.transform, false);
        //            spawnedCard.GetComponent<Draggable>().ChangeEnum(Draggable.Slot.BATTLEHAND);   // change enum to BATTLEHAND

        //            createdCard = spawnedCard.GetComponent<ThisCard>();     // modify their stats to match what they were
        //            if (GlobalControl.Instance.newYourCards[i] != null)
        //            {
        //                createdCard.cost = GlobalControl.Instance.newYourCards[i].cost;
        //                createdCard.attack = GlobalControl.Instance.newYourCards[i].attack;
        //                createdCard.health = GlobalControl.Instance.newYourCards[i].health;
        //                createdCard.golden = GlobalControl.Instance.newYourCards[i].golden;
        //                createdCard.admission = GlobalControl.Instance.newYourCards[i].admission;
        //                createdCard.shield = GlobalControl.Instance.newYourCards[i].shield;
        //                createdCard.cleave = GlobalControl.Instance.newYourCards[i].cleave;
        //                createdCard.guard = GlobalControl.Instance.newYourCards[i].guard;
        //                createdCard.vengeance = GlobalControl.Instance.newYourCards[i].vengeance;
        //                createdCard.command = GlobalControl.Instance.newYourCards[i].command;
        //            }
        //            createdCard.ActivateStasisSlider();


        //            createdCard.ActivateTargetBorder();


        //            yourHand.Add(createdCard);
        //        }
        //        i++;
        //    }
        //}

        for (int i = 0; i < yourCards.Count; i++)
        {
            if (yourCards.Count != GlobalControl.Instance.newYourCards.Count)
                break;


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
                    if (GlobalControl.Instance.newYourCards[i] != null)
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
        /*int i = 0;
        foreach (int unit in GlobalControl.Instance.handCards)
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
        }*/

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

                case 1:
                    {
                        if (enemyFrontlineMinions.Count > 0)
                        {
                            activated = true;
                            StartCoroutine(Attack(0, yourCommander, 2, enemyFrontlineMinions[Random.Range(0, enemyFrontlineMinions.Count)], false));
                            StartCoroutine(Attack(0.01f, yourCommander, 2, enemyFrontlineMinions[Random.Range(0, enemyFrontlineMinions.Count)], false));
                            StartCoroutine(Attack(0.02f, yourCommander, 2, enemyFrontlineMinions[Random.Range(0, enemyFrontlineMinions.Count)], false));
                            StartCoroutine(Attack(0.03f, yourCommander, 2, enemyFrontlineMinions[Random.Range(0, enemyFrontlineMinions.Count)], false));
                        }
                    }
                    break;
                case 2:
                    {
                        if (target != null && target.onField)
                        {
                            activated = true;
                            StartCoroutine(Attack(0, yourCommander, 6, target, false));
                        }
                    }
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
        //GetBoardState();

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
            //createdConsumable = spawnedConsumable.GetComponent<ThisConsumable>();
        }
    }

    public ThisCard SpawnPlayerCard(int id, bool isHandCard)
    {
        if ((isHandCard == false && frontline.transform.childCount <= frontlineSlots) || (isHandCard == true && hand.transform.childCount < 10))
        {
            createdCard = cardTemplate.GetComponent<ThisCard>();
            createdCard.thisId = id;
            spawnedCard = Instantiate(cardTemplate, new Vector3(0, 0, 0), Quaternion.identity);
            createdCard = spawnedCard.GetComponent<ThisCard>();

            if (isHandCard == false)
            {
                spawnedCard.transform.SetParent(frontline.transform, false);
                spawnedCard.GetComponent<Draggable>().ChangeEnum(Draggable.Slot.BOARD);
                createdCard.onField = true;
                frontlineMinions.Add(createdCard);
            }
            else if (isHandCard == true)
            {
                spawnedCard.transform.SetParent(hand.transform, false);
                spawnedCard.GetComponent<Draggable>().ChangeEnum(Draggable.Slot.BATTLEHAND);
                yourHand.Add(createdCard);
            }
        }
        return createdCard;
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
        //PayEnergy(t.energyCost, false);
        backlineMinions.Add(t);
        t.backline = true;
        t.onField = true;
        TriggerOnPlay(t, target);
    }

    public void SpawnToFrontline(GameObject playedCard)
    {
        ThisCard t = playedCard.GetComponent<ThisCard>();
        GameObject spawnedCard;

        if (energy >= t.energyCost && frontlineMinions.Count < frontlineSlots)
        {
            t.copy = true;

            spawnedCard = Instantiate(playedCard);
            Draggable d = spawnedCard.GetComponent<Draggable>();
            spawnedCard.transform.SetParent(frontline.transform);
            spawnedCard.transform.SetSiblingIndex(d.index);
            spawnedCard.transform.localScale = new Vector3(0.50f, 0.50f, 1);
            spawnedCard.GetComponent<CanvasGroup>().blocksRaycasts = true;
            d.enabled = false;

            t = spawnedCard.GetComponent<ThisCard>();

            if (t.admission)
            {
                inactiveMinion = t;
                inactiveMinionBlueprint = playedCard;
                GlobalControl.Instance.targetMode = true;
                SetViableTargets(t);
            }
            else
                ActivateMinion(playedCard, t, null);
        }
    }

    public ThisCard SpawnToBackline(GameObject playedCard, bool isEnemy)
    {
        // ******************* TEST *****************
        GameObject spawnedUnit;
        spawnedUnit = Instantiate(unitTemplate, new Vector3(0, 0, 0), Quaternion.identity);
        // ******************************************

        if (!isEnemy && backlineMinions.Count >= backlineSlots)
            return null;
        if (isEnemy && enemyBacklineMinions.Count >= enemyBacklineSlots)
            return null;

        ThisCard t = playedCard.GetComponent<ThisCard>();
        GameObject spawnedCard;

        GameObject line;
        List<ThisCard> list;
        if (!isEnemy)
        {
            line = backline;
            list = backlineMinions;
        }
        else
        {
            line = enemyBackline;
            list = enemyBacklineMinions;
        }

        t.copy = true;

        spawnedCard = Instantiate(playedCard);
        Draggable d = spawnedCard.GetComponent<Draggable>();
        ThisCard s = spawnedCard.GetComponent<ThisCard>();
        s.abilityList.AddRange(t.abilityList);
        spawnedCard.transform.SetParent(line.transform);
        spawnedCard.transform.SetSiblingIndex(d.index);
        spawnedCard.transform.localScale = new Vector3(0.50f, 0.50f, 1);
        spawnedCard.GetComponent<CanvasGroup>().blocksRaycasts = true;
        d.DisableHighlight();
        d.enabled = false;

        s.DeactivateStasisSlider();

        //if (t.admission)
        //{
        //    inactiveMinion = t;
        //    inactiveMinionBlueprint = playedCard;
        //    GlobalControl.Instance.targetMode = true;
        //    SetViableTargets(t);
        //}
        //else
        //ActivateMinion(playedCard, t, null);

        list.Add(s);
        s.backline = true;
        s.onField = true;

        return s;
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

    ThisCard SummonMinion(int id, bool isEnemy)
    {
        createdCard = cardTemplate.GetComponent<ThisCard>();
        createdCard.thisId = id;

        if (isEnemy == false && frontline.transform.childCount < frontlineSlots || isEnemy == true && enemyFrontline.transform.childCount < frontlineSlots)
        {
            spawnedCard = Instantiate(cardTemplate, new Vector3(0, 0, 0), Quaternion.identity);
            spawnedCard.transform.localScale = new Vector3(0.50f, 0.50f, 1);
            createdCard = spawnedCard.GetComponent<ThisCard>();
            createdCard.onField = true;
            createdCard.GetComponent<Draggable>().enabled = false;

            if (isEnemy == false)
            {
                if (CheckForAbility(createdCard, 2) != null)
                    tauntMinions.Add(createdCard);
                frontlineMinions.Add(createdCard);
                spawnedCard.transform.SetParent(frontline.transform, false);
            }
            else
            {
                if (CheckForAbility(createdCard, 2) != null)
                    enemyTauntMinions.Add(createdCard);
                enemyFrontlineMinions.Add(createdCard);
                spawnedCard.transform.SetParent(enemyFrontline.transform, false);
            }
        }

        if (GlobalControl.Instance.targetMode)
            SetViableTargets(inactiveMinion);

        return createdCard;
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

            case 1:     // slime colony
            case 7:     // infested slime
                {
                    ThisCard summonedCard;
                    //GameObject spawnedReticle;

                    summonedCard = SummonMinion(19, false);
                    summonedCard.attack *= goldenMultiplier;
                    summonedCard.health *= goldenMultiplier;
                    if (playedCard.golden == true)
                        summonedCard.golden = true;

                    //spawnedReticle = Instantiate(reticle, new Vector3(0, 0, 0), Quaternion.identity);
                    //spawnedReticle.transform.SetParent(board.transform.parent);
                }
                break;
            case 2:     // malignant matter
                {
                    StartCoroutine(Attack(0, playedCard, 2, target, false));
                    //StartCoroutine(Attack(0, playedCard, 2, yourCommander, false));
                }
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

                    //for (int i = 0; i < goldenMultiplier; i++)

                    //choicePanel.SetPoolConsumable(1);

                    //SpawnConsumable(Random.Range(1, 6));
                }
                break;
            case 34:    // daniel
                {
                    foreach (ThisCard card in enemies)
                    {
                        playedCard.health++;
                        StartCoroutine(Attack(0, playedCard, 1, card, false));
                    }
                }
                break;
        }
    }

    void TriggerOnPlay(ThisCard playedCard, ThisCard target)
    {
        if (playedCard.admission)
            AdmissionFunctionality(playedCard, target);

        //foreach (ThisCard card in backLineMinions)
        //{
        //    if (card.id == 33)
        //        card.speed += 0.2f;
        //}
    }

    void TriggerOnBuild(ThisCard activatedCard, bool isEnemy)
    {
        //foreach (ThisCard card in yourHand)
        //{
        //    if (card.transform.GetSiblingIndex() > builtCard.transform.GetSiblingIndex())
        //    {
        //        card.energyCost -= 10;
        //    }
        //}

        ThisCard spawnedCard = null;

        List<ThisCard> allyHand = new List<ThisCard>();
        List<ThisCard> enemyHand = new List<ThisCard>();
        List<ThisCard> allyBackline = new List<ThisCard>();
        List<ThisCard> enemyBackline = new List<ThisCard>();

        if (isEnemy == false)
        {
            if (!activatedCard.station)
                spawnedCard = SpawnToBackline(activatedCard.gameObject, false);

            allyHand = yourHand;
            allyBackline = backlineMinions;
            enemyHand = enemyStasisMinions;
            enemyBackline = enemyBacklineMinions;
        }
        else if (isEnemy == true)
        {
            if (!activatedCard.station)
                spawnedCard = SpawnToBackline(activatedCard.gameObject, true);

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

    IEnumerator TriggerOnActivation(ThisCard activatedCard, bool isEnemy)
    {
        ThisCard target = null;
        List<ThisCard> allyRow = new List<ThisCard>();
        List<ThisCard> enemies = new List<ThisCard>();

        if (isEnemy == false)
        {
            if (!activatedCard.backline)
                allyRow.AddRange(frontlineMinions);
            else if (activatedCard.backline)
                allyRow.AddRange(backlineMinions);

            enemies.AddRange(enemyFrontlineMinions);
            enemies.AddRange(enemyBacklineMinions);
        }
        else if (isEnemy == true)
        {
            if (!activatedCard.backline)
                allyRow.AddRange(enemyFrontlineMinions);
            else if (activatedCard.backline)
                allyRow.AddRange(enemyBacklineMinions);

            enemies.AddRange(frontlineMinions);
            enemies.AddRange(backlineMinions);
        }

        // on activation abilities
        for (int i = 0; i < activatedCard.abilityList.Count; i++)
        {
            switch (activatedCard.abilityList[i][0])
            {
                default:
                    break;

                case 6:    // chimera egg: summon a 1/1 slime(19)
                    {
                        SummonMinion(19, isEnemy);
                        break;
                    }
                case 7:    // chonk: give your other minions on the same row 50% speed
                    {
                        foreach (ThisCard card in allyRow)
                        {
                            if (card != activatedCard)
                                card.speed += (float)activatedCard.abilityList[i][1] / 100;
                        }
                        break;
                    }
                case 8:    // symbiotic arm: gain +2 attack
                    {
                        activatedCard.attack += activatedCard.abilityList[i][1];
                        break;
                    }
                //case 25:    // cockroach
                //    {
                //        if (!isEnemy)
                //            SpawnConsumable(7);
                //        break;
                //    }
                case 9:    // squire
                    {
                        foreach (ThisCard card in allyRow)
                        {
                            if (!card.structure)
                            {
                                if (card.transform.GetSiblingIndex() == activatedCard.transform.GetSiblingIndex() + 1)
                                    card.attack++;
                                if (card.transform.GetSiblingIndex() == activatedCard.transform.GetSiblingIndex() - 1)
                                    card.attack++;
                            }
                        }
                        break;
                    }
            }
        }

        // non-rapidfire attack
        if (!activatedCard.rapidFire)
            StartCoroutine(Attack(0, activatedCard, activatedCard.attack, null, isEnemy));

        yield return null;
    }

    IEnumerator Attack(float delay, ThisCard activatedCard, int amount, ThisCard target, bool isEnemy)
    {
        yield return new WaitForSeconds(delay / chargeRate);

        //ThisCard target = null;
        List<ThisCard> allies = new List<ThisCard>();
        List<ThisCard> enemies = new List<ThisCard>();

        if (isEnemy == false)
        {
            allies.AddRange(frontlineMinions);
            allies.AddRange(backlineMinions);
            enemies = enemyBacklineMinions;
        }
        else if (isEnemy == true)
        {
            allies = enemyBacklineMinions;
            enemies.AddRange(frontlineMinions);
            enemies.AddRange(backlineMinions);
        }

        if (amount > 0)
        {
            bool targetIsMinion = true;

            if (isEnemy == false && target == null)
            {
                if (enemyTauntMinions.Count > 0)
                    target = enemyTauntMinions[Random.Range(0, enemyTauntMinions.Count)];
                else if (enemyFrontlineMinions.Count > 0)
                    target = enemyFrontlineMinions[Random.Range(0, enemyFrontlineMinions.Count)];
                else if (!activatedCard.backline)
                {
                    target = enemyCommander;
                    targetIsMinion = false;
                }
            }
            else if (isEnemy == true)
            {
                if (tauntMinions.Count > 0)             // attack taunt minions
                    target = tauntMinions[Random.Range(0, tauntMinions.Count)];
                else if (frontlineMinions.Count > 0)      // attack frontline
                    target = frontlineMinions[Random.Range(0, frontlineMinions.Count)];
                else if (!activatedCard.backline)
                {
                    target = yourCommander;     // attack commander
                    targetIsMinion = false;
                }
            }

            // rapidfire
            if (activatedCard != null && activatedCard.rapidFire && target != null)
                activatedCard.attack -= amount;

            yield return new WaitForSeconds(0.00006f / chargeRate);

            // spawn bullet
            if (target != null && activatedCard != null)
            {
                spawnedBullet = Instantiate(bullet, new Vector3(activatedCard.transform.position.x, activatedCard.transform.position.y, activatedCard.transform.position.z), Quaternion.identity);
                spawnedBullet.transform.SetParent(activatedCard.transform.root);
                spawnedBullet.transform.localScale = new Vector3(1, 1, 0);
                spawnedBullet.GetComponent<Bullet>().StartMoving(target.transform.position.x, target.transform.position.y);
            }

            yield return new WaitForSeconds(0.006f / chargeRate);

            // bullet hits target
            if (target != null)
            {
                DealDamage(activatedCard, amount, target, !isEnemy);
                OnAttackHit(activatedCard, target, targetIsMinion);
            }
        }
    }

    // barrier functionality is in here
    void DealDamage(ThisCard dealer, int amount, ThisCard receiver, bool receiverIsEnemy)
    {
        if (receiver != null)
        {
            if (amount > 0)
            {
                if (CheckForAbility(receiver, 1) == null)   // check for barrier
                {
                    receiver.health -= amount;
                    receiver.ChangeHealthToRed();

                    // take damage abilities
                    for (int i = 0; i < receiver.abilityList.Count; i++)
                    {
                        switch (receiver.abilityList[i][0])
                        {
                            default:
                                break;

                            case 4:
                                SummonMinion(39, receiverIsEnemy);
                                break;
                            case 5:
                                receiver.attack += 1;
                                break;
                        }
                    }
                }
                else
                    RemoveAbility(receiver, 1); // remove barrier

                receiver.GetComponent<ShakeTransformS>().Begin();
            }
        }
    }

    void OnAttackHit(ThisCard card, ThisCard target, bool targetIsMinion)
    {
        //List<int> ability = CheckForAbility(card, 3);

        //if (ability != null)
        //    target.burn += ability[1];

        for (int i = 0; i < card.abilityList.Count; i++)
        {
            switch (card.abilityList[i][0])
            {
                default:
                    break;

                case 3:
                    {
                        if (target != null && targetIsMinion)
                            target.burn += card.abilityList[i][1];
                    }
                    break;
            }
        }
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

    void CheckForDeadMinions()
    {
        List<ThisCard> cardList = new List<ThisCard>();
        cardList.AddRange(frontlineMinions);
        cardList.AddRange(backlineMinions);

        List<ThisCard> enemyCardList = new List<ThisCard>();
        enemyCardList.AddRange(enemyFrontlineMinions);
        enemyCardList.AddRange(enemyBacklineMinions);

        // your minions
        for (int i = 0; i < cardList.Count; i++)
        {
            ThisCard card = cardList[i];

            if (card.health <= 0)
            {
                if (card.vengeance == true)
                    VengeanceStack(card, false);

                if (card.burn > 0)
                {
                    if (i != 0)
                        cardList[i - 1].burn += card.burn;
                    if (i < cardList.Count - 1)
                        cardList[i + 1].burn += card.burn;
                }

                cardList.Remove(card);
                frontlineMinions.Remove(card);
                backlineMinions.Remove(card);
                tauntMinions.Remove(card);
                if (card)
                    Destroy(card.gameObject);
                //Destroy(card.gameObject, battleSpeed / 2);
            }
        }

        // enemy minions
        for (int i = 0; i < enemyCardList.Count; i++)
        {
            ThisCard card = enemyCardList[i];

            if (card.health <= 0)
            {
                if (card.vengeance == true)
                    VengeanceStack(card, true);

                if (card.burn > 0)
                {
                    if (i != 0)
                        enemyCardList[i - 1].burn += card.burn;
                    if (i < enemyCardList.Count - 1)
                        enemyCardList[i + 1].burn += card.burn;
                    //foreach (ThisCard minion in enemyMinions)
                    //{
                    //    if (minion.transform.GetSiblingIndex() == card.transform.GetSiblingIndex() - 1 || minion.transform.GetSiblingIndex() == card.transform.GetSiblingIndex() + 1)
                    //        minion.burn += card.burn;
                    //}
                }

                enemyCardList.Remove(card);
                enemyFrontlineMinions.Remove(card);
                enemyBacklineMinions.Remove(card);
                enemyTauntMinions.Remove(card);
                if (card)
                    Destroy(card.gameObject);
                //Destroy(card.gameObject, battleSpeed / 2);
            }
        }
    }

    void VengeanceStack(ThisCard deadCard, bool isEnemy)
    {
        if (isEnemy == false)
            yourVengeances.Add(deadCard);
        else
            enemyVengeances.Add(deadCard);
    }

    void VengeanceFunctionality(bool isEnemy)
    {
        //GetBoardState();

        int id = 0;
        bool golden = false;
        int goldenMultiplier = 1;
        if (isEnemy == false && yourVengeances.Count != 0)
        {
            id = yourVengeances[0].id;
            golden = yourVengeances[0].golden;
        }
        else if (isEnemy == true && enemyVengeances.Count != 0)
        {
            id = enemyVengeances[0].id;
            golden = enemyVengeances[0].golden;
        }

        if (golden == true)
            goldenMultiplier = 2;

        List<ThisCard> minionList;
        if (isEnemy == false)
            minionList = frontlineMinions;
        else
            minionList = enemyBacklineMinions;

        ThisCard receiver = null;

        switch (id)
        {
            default:
                break;

            case 20:    // chimera egg
                ThisCard summon;
                for (int i = 0; i < 2; i++)
                {
                    //GetBoardState();
                    //if (minionList.Count < 7)
                    //{
                    summon = SummonMinion(1, isEnemy);




                    //summon.transform.SetSiblingIndex(0);




                    summon.attack *= goldenMultiplier;
                    summon.health *= goldenMultiplier;
                    if (golden == true)
                        summon.golden = true;
                    //}
                }
                break;

            case 26:    // volatile crawler
                if (isEnemy == false)
                {
                    if (enemyBacklineMinions.Count != 0)
                        receiver = enemyBacklineMinions[Random.Range(0, enemyBacklineMinions.Count)];
                }
                else
                {
                    if (frontlineMinions.Count != 0)
                        receiver = frontlineMinions[Random.Range(0, frontlineMinions.Count)];
                }

                if (receiver != null)
                {
                    //for (int i = 0; i < goldenMultiplier; i++)
                    //    DealDamage(null, 4, receiver, false);
                }
                break;

            case 21:    // power core
                foreach (ThisCard x in minionList)
                {
                    x.attack += 1 * goldenMultiplier;
                    x.health += 1 * goldenMultiplier;
                }
                break;

            case 28:    // selfless hero
                if (minionList.Count != 0)
                {
                    for (int i = 0; i < goldenMultiplier; i++)
                    {
                        List<ThisCard> shieldlessMinions = new List<ThisCard>();

                        foreach (ThisCard card in minionList)
                        {
                            if (card.shield == false)
                                shieldlessMinions.Add(card);
                        }

                        if (shieldlessMinions.Count != 0)
                        {
                            receiver = shieldlessMinions[Random.Range(0, shieldlessMinions.Count)];
                            receiver.shield = true;
                        }
                    }
                }
                break;
        }

        if (isEnemy == false && yourVengeances.Count != 0)
            yourVengeances.RemoveAt(0);
        else if (isEnemy == true && enemyVengeances.Count != 0)
            enemyVengeances.RemoveAt(0);
    }

    void BattleWon()
    {
        GlobalControl.Instance.turnNumber++;
        GlobalControl.Instance.lives = lives;
        endBattle = true;
    }

    void BattleLost(int totalDamage)
    {
        lives -= totalDamage;
        if (lives <= 0)
            RestartButton();
        GlobalControl.Instance.lives = lives;
        endBattle = true;
    }

    private void ResetStartButton()
    {
        speedUpButton.SetActive(false);
        startButton.SetActive(true);
    }

    // button
    public void StartBattle()
    {
        //foreach (Transform card in board.transform)     // disable Draggable from your cards
        //    card.GetComponent<Draggable>().enabled = false;

        timeLeft = 1000;
        pauseBattle = false;
        StartCoroutine(BattleSequence());    // start combat

        speedUpButton.SetActive(true);      // change start button to speed-up button
        startButton.SetActive(false);
    }

    // button
    public void AttackButton()
    {
        if (!pauseBattle)
        {
            for (int i = 0; i < backlineMinions.Count; i++)
            {
                ThisCard card = backlineMinions[i];
                if (!card.structure)
                {
                    card.backline = false;
                    card.transform.SetParent(frontline.transform);
                    frontlineMinions.Add(card);
                }

                if (CheckForAbility(card, 2) != null)
                    tauntMinions.Add(card);
            }

            backlineMinions.RemoveAll(item => !item.backline);
        }
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
