using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDB : MonoBehaviour
{
    public static Dictionary<int, Card> cardList;
    public static Dictionary<int, List<int>> abilityDB;
    private Sprite cardBackground;

    public List<List<int>> abilityList = new List<List<int>>();

    void Awake()
    {
        cardList = new Dictionary<int, Card>();
        abilityDB = new Dictionary<int, List<int>>();
        cardBackground = Resources.Load<Sprite>("grey");

        // **** ABILITIES ****

        abilityDB.Add(0, CreateAbility(0, 0, 0));   // none

        abilityDB.Add(1, CreateAbility(1, 0, 0));   // barrier
        abilityDB.Add(2, CreateAbility(2, 0, 0));   // taunt

        abilityDB.Add(3, CreateAbility(3, 2, 0));   // on minion hit: 2 burn

        abilityDB.Add(4, CreateAbility(4, 0, 0));   // on take damage: summon a 1/3 taunt minion
        abilityDB.Add(5, CreateAbility(5, 0, 0));   // on take damage: gain +1 attack

        abilityDB.Add(6, CreateAbility(6, 0, 0));   // on activation: summon a 1/1 slime(19)
        abilityDB.Add(7, CreateAbility(7, 50, 0));  // on activation: give your other minions 50% speed
        abilityDB.Add(8, CreateAbility(8, 2, 0));   // on activation: gain +2 attack
        abilityDB.Add(9, CreateAbility(9, 1, 0));   // on activation: adjacent minions gain +1 attack

        abilityDB.Add(10, CreateAbility(10, 1, 0));   // on build: give your other blueprints +1 health
        abilityDB.Add(11, CreateAbility(11, 0, 0));   // on build: give your other backline minions barrier
        abilityDB.Add(12, CreateAbility(12, 1, 0));   // on build: give your other chimera blueprints +1/+1
        abilityDB.Add(15, CreateAbility(15, 20, 0));   // on build: gain 20 energy
        abilityDB.Add(16, CreateAbility(16, 20, 0));   // on build: advance adjacent cards by 20

        abilityDB.Add(13, CreateAbility(13, 0, 0));   // admission: summon a 1/1 slime(19)
        abilityDB.Add(14, CreateAbility(14, 1, 0));   // admission: give your other defects +1 health

        // **** CARDS ****

        //                              GameObject, int, string, list,                  string, int,  int,   float,      bool,    int,    int,    float,     string, Sprite,                                      string
        //                              gameObject, Id,  Name,   Abilities,             Text,   Tier, Cost,  EnergyCost, Station, Attack, Health, BuildTime, Type,   ThisImage,                                   Color
        cardList.Add(0, Card.MakeObject(gameObject, 0,   "None", AddAbilities(0, 0, 0), "None", 0,    3,     100,        false,   0,      1,      1,         "None", Resources.Load<Sprite>("Card_Images/smorc"), "Red"));
        // TIER 1
        cardList.Add(1, Card.MakeObject(gameObject, 1, "Slime Colony", AddAbilities(13, 0, 0), "Admission: Summon a 1/1 Chimera", 1, 3, 80, false, 15, 50, 7, "Chimera", Resources.Load<Sprite>("Card_Images/smorc"), "Green"));
        cardList.Add(2, Card.MakeObject(gameObject, 2, "Malignant Matter", AddAbilities(2, 0, 0), "Taunt. Admission: Deal 2 damage to an enemy Minion", 1, 3, 100, false, 20, 110, 10, "Defect", Resources.Load<Sprite>("Card_Images/malignant_matter"), "Blue"));
        cardList.Add(3, Card.MakeObject(gameObject, 3, "Sentinel Scout", AddAbilities(1, 0, 0), "Barrier. Admission: Give a friendly Sentinel +2/+2", 1, 3, 120, false, 20, 150, 12, "Sentinel", Resources.Load<Sprite>("Card_Images/lancer"), "Red"));
        //cardList.Add(4, Card.MakeObject(gameObject, 4, "Junkbot", AddAbilities(14, 0, 0), "Admission: Give your other Defects +1 health", 1, 3, 70, false, 2, 5, 3, "Defect", cardBackground, "Blue"));
        //cardList.Add(20, Card.MakeObject(gameObject, 20, "Chimera Egg", AddAbilities(6, 0, 0), "Active: Summon a 1/1 Chimera", 1, 3, 100, false, 0, 7, 5, "Chimera", Resources.Load<Sprite>("Card_Images/eggo"), "Green"));
        //cardList.Add(23, Card.MakeObject(gameObject, 23, "Joe", AddAbilities(3, 0, 0), "Attacks apply 2 Burn", 1, 3, 100, false, 1, 6, 4, "Sentinel", Resources.Load<Sprite>("Card_Images/burn"), "Red"));
        //cardList.Add(24, Card.MakeObject(gameObject, 24, "Symbiotic Arm", AddAbilities(8, 0, 0), "Rapid-Fire. Active: This gains +2 attack", 1, 3, 100, false, 4, 5, 4, "Defect", Resources.Load<Sprite>("Card_Images/symbiotic_arm"), "Blue"));
        //cardList.Add(25, Card.MakeObject(gameObject, 25, "Cockroach", AddAbilities(0, 0, 0), "Admission and Active: Add a Borger to your hand", 1, 3, 100, false, 2, 8, 5, "Chimera", Resources.Load<Sprite>("Card_Images/cockroach"), "Green"));
        //cardList.Add(27, Card.MakeObject(gameObject, 27, "Alchemist", AddAbilities(10, 0, 0), "On Build: Give your other Blueprints +1 health", 1, 3, 100, false, 2, 6, 5, "", cardBackground, "Grey"));
        //cardList.Add(28, Card.MakeObject(gameObject, 28, "Selfless Hero", AddAbilities(0, 0, 0), "Vengeance: Give a random friendly Minion Barrier", 1, 3, 100, false, 2, 4, 3, "", cardBackground, "Grey"));
        //cardList.Add(30, Card.MakeObject(gameObject, 30, "Chonk", AddAbilities(7, 0, 0), "Active: Give your other Minions 50% speed. This gains an extra +1/+1 from Borgers", 1, 3, 150, false, 1, 10, 4, "Chimera", cardBackground, "Green"));
        //cardList.Add(33, Card.MakeObject(gameObject, 33, "Generator", AddAbilities(15, 0, 0), "Active: Gain 10 Energy", 1, 3, 60, true, 0, 0, 2, "Structure", Resources.Load<Sprite>("Card_Images/generator"), "Grey"));
        //cardList.Add(34, Card.MakeObject(gameObject, 34, "Daniel Plainview", AddAbilities(0, 0, 0), "Admission: Steal 1 Health from all enemy Minions", 1, 3, 150, false, 3, 4, 3, "Defect", cardBackground, "Blue"));
        //cardList.Add(37, Card.MakeObject(gameObject, 37, "Squire", AddAbilities(9, 0, 0), "Active: Give adjacent Minions +1 attack", 1, 3, 100, false, 1, 7, 3, "Sentinel", cardBackground, "Red"));
        //cardList.Add(38, Card.MakeObject(gameObject, 38, "Security Rover", AddAbilities(4, 0, 0), "Whenever this takes damage, summon a 1/3 minion with taunt", 1, 3, 100, false, 2, 6, 3, "Sentinel", cardBackground, "Red"));
        //cardList.Add(40, Card.MakeObject(gameObject, 40, "Mobile barrier", AddAbilities(11, 2, 0), "Taunt. On build: Give your other backline Minions Barrier", 1, 3, 100, false, 0, 5, 3, "", cardBackground, "Grey"));
        //cardList.Add(41, Card.MakeObject(gameObject, 41, "Fungus Farms", AddAbilities(12, 0, 0), "On build: Give your other Chimera Blueprints +1/+1", 1, 3, 70, true, 0, 0, 3, "Chimera", cardBackground, "Green"));
        //cardList.Add(42, Card.MakeObject(gameObject, 42, "Catalyst", AddAbilities(16, 0, 0), "On build: Advance adjacent cards by 20", 1, 3, 70, true, 0, 0, 3, "", cardBackground, "Grey"));
        //// TIER 2
        //cardList.Add(21, Card.MakeObject(gameObject, 21, "Power Core", "Vengeance: Give your Minions +1/+1", 2, 3, 1, 1, 1, "", Resources.Load<Sprite>("Card_Images/power_core"), "Grey", false, false, false, false, false, true, false));
        //cardList.Add(22, Card.MakeObject(gameObject, 22, "Janitor", "After you play a Sentinel, give another random friendly Sentinel +1/+1", 2, 3, 1, 2, 1, "Sentinel", Resources.Load<Sprite>("Card_Images/janitor"), "Red", false, false, false, false, false, false, false));
        //cardList.Add(4, Card.MakeObject(gameObject, 4, "Security Bot", "Divine Shield.", 2, 3, 2, 3, 1, "Sentinel", Resources.Load<Sprite>("Card_Images/shield_guy"), "Red", false, false, true, false, false, false, false));
        //cardList.Add(5, Card.MakeObject(gameObject, 5, "Junkbot", "Admission: Give your other Defects +1 health", 2, 3, 3, 2, 1, "Defect", cardBackground, "Blue", false, true, false, false, false, false, false));
        //cardList.Add(6, Card.MakeObject(gameObject, 6, "Temp Chimera #1", "Admission: Give your other Chimeras +2 attack", 2, 3, 100, 10, 2, 3, 3, "Chimera", cardBackground, "Green", false, false, true, false, false, false, false, false, false));
        //cardList.Add(7, Card.MakeObject(gameObject, 7, "Infested Slime", "Admission: Summon a 1/1 Chimera", 2, 3, 3, 3, 1, "Defect", cardBackground, "Blue", false, true, false, false, false, false, false));
        //cardList.Add(26, Card.MakeObject(gameObject, 26, "Volatile Crawler", "Vengeance: Deal 4 damage to a random enemy Minion", 2, 3, 2, 2, 1, "Defect", Resources.Load<Sprite>("Card_Images/volatile_crawler"), "Blue", false, false, false, false, false, true, false));
        //cardList.Add(31, Card.MakeObject(gameObject, 31, "Writhing Mass", "After you play a Defect, gain +1/+1", 2, 3, 2, 3, 1, "Defect", cardBackground, "Blue", false, false, false, false, false, false, false));
        //cardList.Add(32, Card.MakeObject(gameObject, 32, "Resupplier", "At the start of turn, add a Banana to your hand", 2, 3, 3, 1, 1, "Sentinel", cardBackground, "Red", false, false, false, false, false, false, false));
        //// TIER 3
        //cardList.Add(8, Card.MakeObject(gameObject, 8, "Mantipede", "Cleave.", 3, 3, 2, 5, 1, "Chimera", cardBackground, "Green", false, false, false, true, false, false, false));
        //cardList.Add(9, Card.MakeObject(gameObject, 9, "Temp Chimera #2", "", 3, 3, 4, 5, 1, "Chimera", cardBackground, "Green", false, false, false, false, false, false, false));
        //cardList.Add(10, Card.MakeObject(gameObject, 10, "Temp Defect #2", "Divine Shield.", 3, 3, 2, 8, 1, "Defect", cardBackground, "Blue", false, false, true, false, false, false, false));
        //cardList.Add(11, Card.MakeObject(gameObject, 11, "Temp Sentinel #2", "Divine Shield.", 3, 3, 4, 3, 1, "Sentinel", Resources.Load<Sprite>("Card_Images/heavy_guy"), "Red", false, false, true, false, false, false, false));
        //// TIER 4
        //cardList.Add(12, Card.MakeObject(gameObject, 12, "Temp Defect #4", "Admission: Give your other Defects +4 attack", 4, 3, 4, 4, 1, "Defect", cardBackground, "Blue", false, true, false, false, false, false, false));
        //cardList.Add(13, Card.MakeObject(gameObject, 13, "Temp Chimera #3", "", 4, 3, 7, 4, 1, "Chimera", cardBackground, "Green", false, false, false, false, false, false, false));
        //cardList.Add(14, Card.MakeObject(gameObject, 14, "Temp Sentinel #3", "", 4, 3, 4, 8, 1, "Sentinel", cardBackground, "Red", false, false, false, false, false, false, false));
        //cardList.Add(15, Card.MakeObject(gameObject, 15, "Temp Sentinel #4", "Divine Shield.", 4, 3, 3, 9, 1, "Sentinel", cardBackground, "Red", false, false, true, false, false, false, false));
        //// TIER 5
        //cardList.Add(16, Card.MakeObject(gameObject, 16, "Guardian Titan", "Divine Shield.", 5, 3, 7, 7, 1, "Sentinel", cardBackground, "Red", false, false, true, false, false, false, false));
        //cardList.Add(17, Card.MakeObject(gameObject, 17, "Gigantic Hydra", "Cleave.", 5, 3, 5, 7, 1, "Chimera", cardBackground, "Green", false, false, false, true, false, false, false));
        //cardList.Add(18, Card.MakeObject(gameObject, 18, "Horrendous Growth", "Admission: Destroy your other Minions and gain their stats", 5, 3, 1, 1, 1, "Defect", cardBackground, "Blue", false, true, false, false, false, false, false));
        //// TIER 6
        //cardList.Add(29, Card.MakeObject(gameObject, 29, "Goose", "After you play a Minion with Admission, give your Defects +1/+1", 6, 3, 4, 12, 1, "Defect", cardBackground, "Blue", false, false, false, false, false, false, false));
        // NOT IN SHOP POOL
        cardList.Add(19, Card.MakeObject(gameObject, 19, "Slime", AddAbilities(0, 0, 0), "", 0, 3, 50, false, 3, 15, 5, "Chimera", cardBackground, "Green"));
        cardList.Add(35, Card.MakeObject(gameObject, 35, "Turret", AddAbilities(0, 0, 0), "", 0, 3, 100, false, 1, 10, 10, "Structure", Resources.Load<Sprite>("Card_Images/turret"), "Grey"));
        cardList.Add(36, Card.MakeObject(gameObject, 36, "Assault Drone", AddAbilities(0, 0, 0), "", 0, 3, 100, false, 20, 100, 10, "", Resources.Load<Sprite>("Card_Images/drone"), "Grey"));
        cardList.Add(39, Card.MakeObject(gameObject, 39, "Guard Bot", AddAbilities(2, 0, 0), "Taunt", 0, 3, 100, false, 1, 3, 10, "Sentinel", cardBackground, "Red"));
    }

    private List<int> CreateAbility(int a, int b, int c)
    {
        List<int> ability = new List<int>();
        ability.Add(a);
        ability.Add(b);
        ability.Add(c);

        return ability;
    }

    private List<List<int>> AddAbilities(int a, int b, int c)
    {
        List<List<int>> abilities = new List<List<int>>();

        if (a != 0)
            abilities.Add(abilityDB[a]);
        if (b != 0)
            abilities.Add(abilityDB[b]);
        if (c != 0)
            abilities.Add(abilityDB[c]);

        return abilities;
    }
}
