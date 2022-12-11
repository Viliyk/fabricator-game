using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Card : MonoBehaviour
{
    public int id;
    public string cardName;
    public string cardDescription;
    public int tier;
    public int cost;
    public int attack;
    public int health;
    public float buildTime;
    public string type;
    public Sprite thisImage;
    public string color;

    public float energyCost;

    public bool station;

    public List<List<int>> abilityList = new List<List<int>>();

    // constructor that allows cards to be added in the database
    public static Card MakeObject(GameObject gameObject, int Id, string CardName, List<List<int>> AbilityList, string CardDescription, int Tier, int Cost,
        float EnergyCost, bool Station, int Attack, int Health, float buildTime, string Type, Sprite ThisImage, string Color)
    {
        Card obj = gameObject.AddComponent<Card>();
        obj.id = Id;
        obj.cardName = CardName;
        obj.abilityList = AbilityList;
        obj.cardDescription = CardDescription;
        obj.tier = Tier;
        obj.cost = Cost;
        obj.energyCost = EnergyCost;
        obj.station = Station;
        obj.attack = Attack;
        obj.health = Health;
        obj.buildTime = buildTime;
        obj.type = Type;
        obj.thisImage = ThisImage;
        obj.color = Color;
        return obj;
    }
}
