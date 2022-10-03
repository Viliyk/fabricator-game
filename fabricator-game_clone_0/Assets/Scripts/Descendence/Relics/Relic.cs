using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Relic : MonoBehaviour
{
    public int id;
    public string relicName;
    public string relicDescription;
    public int tier;
    public string type;
    public Sprite thisImage;
    public string color;
    public bool active;
    public int cost;

    // constructor that allows cards to be added in the database
    public static Relic MakeObject(GameObject gameObject, int Id, string RelicName, string RelicDescription, int Tier, string Type,
        Sprite ThisImage, string Color, bool Active, int Cost)
    {
        Relic obj = gameObject.AddComponent<Relic>();
        obj.id = Id;
        obj.relicName = RelicName;
        obj.relicDescription = RelicDescription;
        obj.tier = Tier;
        obj.type = Type;
        obj.thisImage = ThisImage;
        obj.color = Color;
        obj.active = Active;
        obj.cost = Cost;
        return obj;
    }
}
