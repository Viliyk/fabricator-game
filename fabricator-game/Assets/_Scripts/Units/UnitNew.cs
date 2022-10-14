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
        }

        [Space(15)]
        [Header("Unit Settings")]

        public UnitType type;
        public bool isPlayerUnit;
        public string unitName;
        public GameObject unitPrefab;

        [Space(15)]
        [Header("Unit Base Stats")]
        [Space(30)]

        public int cost;
        public int attack;
        public float attackRange;
        public int health;
        public int armor;
    }
}
