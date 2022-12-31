using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceDB : MonoBehaviour
{
    public static Dictionary<int, Choice> choiceList;
    private Sprite cardBackground;

    void Awake()
    {
        choiceList = new Dictionary<int, Choice>();
        cardBackground = Resources.Load<Sprite>("ratio34blank");

        //                                  GameObject, int, string, string, string, Sprite,                                      int,  int
        //                                  gameObject, Id,  Name,   Text,   Color,  ThisImage,                                   Tier, Cost
        choiceList.Add(0, Choice.MakeObject(gameObject, 0,  "None", "None", "Red",   Resources.Load<Sprite>("Card_Images/smorc"), 0,    0));
        // TIER 1
        choiceList.Add(1, Choice.MakeObject(gameObject, 1, "Repair", "Pay 1 gold to restore 10 lives", "Green", cardBackground, 1, 1));
        choiceList.Add(2, Choice.MakeObject(gameObject, 2, "Shop", "Go to a shop where you can buy and sell cards", "Blue", cardBackground, 1, 0));
        choiceList.Add(3, Choice.MakeObject(gameObject, 3, "Enemy", "Fight an enemy and get rewarded with 3 gold", "Red", Resources.Load<Sprite>("Card_Images/smorc"), 1, 0));
    }
}
