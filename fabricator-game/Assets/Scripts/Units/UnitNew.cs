using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fabricator.Units
{
    [CreateAssetMenu(fileName = "New Unit", menuName = "Create New Unit")]

    public class UnitNew : ScriptableObject
    {
        public enum UnitType
        {
            Worker,
            Warrior,
            Healer
        };
        public UnitType type;

        public bool isPlayerUnit;
        public string unitName;
        public GameObject unitPrefab;

        public int cost;
        public int attack;
        public int health;
        public int armor;
    }
}
