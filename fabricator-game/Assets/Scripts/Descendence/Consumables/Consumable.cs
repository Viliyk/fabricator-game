using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Consumable : MonoBehaviour
{
    public int id;
    public string cardName;
    public string cardDescription;
    public int tier;
    public string type;
    public Sprite thisImage;
    public string color;

    // constructor that allows cards to be added in the database
    public static Consumable MakeObject(GameObject gameObject, int Id, string CardName, string CardDescription, int Tier, string Type,
        Sprite ThisImage, string Color)
    {
        Consumable obj = gameObject.AddComponent<Consumable>();
        obj.id = Id;
        obj.cardName = CardName;
        obj.cardDescription = CardDescription;
        obj.tier = Tier;
        obj.type = Type;
        obj.thisImage = ThisImage;
        obj.color = Color;
        return obj;
    }
}