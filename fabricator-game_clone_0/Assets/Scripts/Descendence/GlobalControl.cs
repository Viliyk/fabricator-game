using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl Instance;

    public ShopManager shopManager;
    public BattleManager battleManager;

    public bool debugMode;

    public bool targetMode;

    public GameObject buyZone;
    public GameObject useZone;
    public GameObject shopHighlight;
    public GameObject boardHighlight;
    public GameObject frontlineHighlight;
    public GameObject backlineHighlight;

    public int shopLevel;
    public int gold;
    public int upgradeCost;
    public int nextUpgrade;
    public int turnNumber;
    public int lives;
    public int maxLives;
    public int credits;
    public int pathNumber;
    public int currentNode;
    public bool isLocked;
    public int[] yourCards;
    public List<Container> newYourCards;
    public List<Container> newHandCards;
    public Container[] containers;
    public int[] handCards;
    public int[] lockedCards;

    public List<int> shopConsumables;
    public List<int> battleConsumables;
    public List<int> yourItems;
    public List<int> ownedRelics;

    void Awake()
    {
        // makes sure there is always exactly one global control active
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
            Destroy(gameObject);

        // starting values
        debugMode = false;

        targetMode = false;

        shopLevel = 1;
        gold = 6;
        upgradeCost = 6;
        nextUpgrade = 7;
        yourCards = new int[7];
        newYourCards = new List<Container>();
        newHandCards = new List<Container>();
        handCards = new int[10];
        lockedCards = new int[7];
        isLocked = false;
        turnNumber = 1;
        lives = 100;
        maxLives = 100;
        credits = 100;
        pathNumber = 0;


        shopConsumables = new List<int>();
        battleConsumables = new List<int>();
        yourItems = new List<int>();
        ownedRelics = new List<int>();





        yourCards[0] = 36;
        yourCards[1] = 36;
        yourCards[2] = 36;
    }

    public void EmptyContainers()
    {
        if (turnNumber > 0)
        {
            newYourCards.Clear();
            newHandCards.Clear();
            shopConsumables.Clear();
            battleConsumables.Clear();
            yourItems.Clear();

            containers = new Container[17];
            containers = GetComponents<Container>();

            int a = containers.Length;

            for (int i = 0; i < a; i++)
            {
                if (containers[i] != null)
                    Destroy(containers[i]);
            }
        }
    }
}
