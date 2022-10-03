using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicDB : MonoBehaviour
{
    public static Dictionary<int, Relic> relicList;
    private Sprite cardBackground;

    void Awake()
    {
        relicList = new Dictionary<int, Relic>();
        cardBackground = Resources.Load<Sprite>("Square");

        //                                GameObject, int, string, string, int,   string, Sprite,                           string, bool,   int
        //                                gameObject, Id,  Name,   Text,   Tier,  Type,   ThisImage,                        Color,  Active, Cost 
        relicList.Add(0, Relic.MakeObject(gameObject, 0,   "None", "None", 0,     "None", Resources.Load<Sprite>("Square"), "Red",  false,  0));

        relicList.Add(1, Relic.MakeObject(gameObject, 1, "Gold Fountain", "At the start of your turn add a Coin to your hand", 1, "Shop", cardBackground, "White", false, 0));
        relicList.Add(2, Relic.MakeObject(gameObject, 2, "Corruption", "When you buy a Defect, give it +1/+1", 1, "Shop", cardBackground, "Blue", false, 0));
        relicList.Add(3, Relic.MakeObject(gameObject, 3, "Dogfood", "When you buy a Chimera, give it +1/+1", 1, "Shop", cardBackground, "Green", false, 0));
        relicList.Add(4, Relic.MakeObject(gameObject, 4, "Capacitor", "When you buy a Sentinel, give it +1 attack", 1, "Shop", cardBackground, "Red", false, 0));
        relicList.Add(5, Relic.MakeObject(gameObject, 5, "Diamond Hands", "Cost: 1g. Add a Coin to your hand. Can be used any number of times", 1, "Shop", cardBackground, "White", true, 1));
        relicList.Add(6, Relic.MakeObject(gameObject, 6, "Borger Recipe", "Cost: 0g. Transform all cards in your hand into Borgers", 1, "Shop", cardBackground, "Green", true, 0));
    }
}
