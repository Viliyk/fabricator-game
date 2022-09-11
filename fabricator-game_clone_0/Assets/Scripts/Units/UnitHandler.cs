using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fabricator.Units
{
    public class UnitHandler : MonoBehaviour
    {
        public static UnitHandler instance;

        [SerializeField]
        private UnitNew unit, unit2;
        public Transform playerUnits;

        void Start()
        {
            instance = this;

            GameObject spawnedUnit = Instantiate(unit.unitPrefab, transform.position, Quaternion.identity, playerUnits);
            GameObject spawnedUnit2 = Instantiate(unit2.unitPrefab, transform.position, Quaternion.identity, playerUnits);
        }
    }
}
