using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Container : MonoBehaviour
{
    public int cost;
    public int attack;
    public int health;
    public bool golden;
    public bool admission;
    public bool shield;
    public bool cleave;
    public bool guard;
    public bool vengeance;
    public bool command;

    public static Container SetValues(GameObject gameObject, int Cost, int Attack, int Health, bool Golden, bool OnPlay, bool Shield,
        bool Cleave, bool Guard, bool Vengeance, bool Command)
    {
        Container obj = gameObject.AddComponent<Container>();
        obj.cost = Cost;
        obj.attack = Attack;
        obj.health = Health;
        obj.golden = Golden;
        obj.admission = OnPlay;
        obj.shield = Shield;
        obj.cleave = Cleave;
        obj.guard = Guard;
        obj.vengeance = Vengeance;
        obj.command = Command;
        return obj;
    }
}
