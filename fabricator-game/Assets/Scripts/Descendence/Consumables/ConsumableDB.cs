using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableDB : MonoBehaviour
{
    public static Dictionary<int, Consumable> consumableList;
    private Sprite cardBackground;

    void Awake()
    {
        consumableList = new Dictionary<int, Consumable>();
        cardBackground = Resources.Load<Sprite>("Square");

        //                                          GameObject, int, string, string, int,   string, Sprite,                           string
        //                                          gameObject, Id,  Name,   Text,   Tier,  Type,   ThisImage,                        Color
        consumableList.Add(0, Consumable.MakeObject(gameObject, 0,   "None", "None", 0,     "None", Resources.Load<Sprite>("Square"), "Red"));

        consumableList.Add(1, Consumable.MakeObject(gameObject, 1, "Coin", "Gain 1 Gold", 1, "Shop", cardBackground, "Red"));
        consumableList.Add(2, Consumable.MakeObject(gameObject, 2, "Wide Buff", "Give your Minions +1/+1", 1, "Battle", cardBackground, "Blue"));
        consumableList.Add(3, Consumable.MakeObject(gameObject, 3, "Guard Potion", "Give any Minion Guard", 1, "Hybrid", cardBackground, "White"));
        consumableList.Add(4, Consumable.MakeObject(gameObject, 4, "Banana", "Give a friendly Minion +1/+1", 1, "Shop", cardBackground, "Red"));
        consumableList.Add(5, Consumable.MakeObject(gameObject, 5, "Defected Banana", "Give a friendly Minion +3/+3", 1, "Battle", cardBackground, "Blue"));
        consumableList.Add(6, Consumable.MakeObject(gameObject, 6, "Golden Ticket", "Choose a Minion from one tier higher than your shop tier", 0, "Shop", cardBackground, "Red"));
        consumableList.Add(7, Consumable.MakeObject(gameObject, 7, "Borger", "Give a friendly Chimera +1/+1, and a smile", 0, "Shop", cardBackground, "Red"));
    }
}
