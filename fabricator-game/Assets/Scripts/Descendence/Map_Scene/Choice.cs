using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Choice : MonoBehaviour
{
    public int id;
    public string cardName;
    public string cardDescription;
    public string color;
    public Sprite thisImage;
    public int tier;
    public int cost;

    // constructor that allows cards to be added in the database
    public static Choice MakeObject(GameObject gameObject, int Id, string CardName, string CardDescription, string Color, Sprite ThisImage, int Tier, int Cost)
    {
        Choice obj = gameObject.AddComponent<Choice>();
        obj.id = Id;
        obj.cardName = CardName;
        obj.cardDescription = CardDescription;
        obj.color = Color;
        obj.thisImage = ThisImage;
        obj.tier = Tier;
        obj.cost = Cost;
        return obj;
    }
}
