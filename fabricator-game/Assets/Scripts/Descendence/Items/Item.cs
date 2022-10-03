using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Item : MonoBehaviour
{
    public int id;
    public string cardName;
    public string cardDescription;
    public string color;
    public Sprite thisImage;
    public int tier;
    public int cost;
    public float energyCost;
    public float timeCost;

    // constructor that allows cards to be added in the database
    public static Item MakeObject(GameObject gameObject, int Id, string CardName, string CardDescription, string Color, Sprite ThisImage, int Tier, int Cost,
        float EnergyCost, float TimeCost)
    {
        Item obj = gameObject.AddComponent<Item>();
        obj.id = Id;
        obj.cardName = CardName;
        obj.cardDescription = CardDescription;
        obj.color = Color;
        obj.thisImage = ThisImage;
        obj.tier = Tier;
        obj.cost = Cost;
        obj.energyCost = EnergyCost;
        obj.timeCost = TimeCost;
        return obj;
    }
}

