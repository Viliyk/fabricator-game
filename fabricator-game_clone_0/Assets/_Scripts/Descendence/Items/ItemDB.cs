using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB : MonoBehaviour
{
    public static Dictionary<int, Item> itemList;
    private Sprite cardBackground;

    void Awake()
    {
        itemList = new Dictionary<int, Item>();
        cardBackground = Resources.Load<Sprite>("Square");

        //                              GameObject, int, string, string, string, Sprite,                           int,  int,  int,        int
        //                              gameObject, Id,  Name,   Text,   Color,  ThisImage,                        tier, cost, energyCost, timeCost
        itemList.Add(0, Item.MakeObject(gameObject, 0,   "None", "None", "Red",  Resources.Load<Sprite>("Square"), 0,    0,    0,          0));

        itemList.Add(1, Item.MakeObject(gameObject, 1, "Side Cannons", "Deal 2 damage to a random enemy Minion 4 times", "Grey", Resources.Load<Sprite>("Square"), 1, 3, 50, 10));
        itemList.Add(2, Item.MakeObject(gameObject, 2, "Precision Railgun", "Deal 6 damage to a Minion", "Blue", Resources.Load<Sprite>("Square"), 1, 3, 50, 10));
    }
}
