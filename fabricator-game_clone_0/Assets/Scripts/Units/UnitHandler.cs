using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fabricator.Units
{
    public class UnitHandler : MonoBehaviour
    {
        public UnitNew unit;
        public Transform playerUnits;

        void Start()
        {
            GameObject spawnedUnit = Instantiate(unit.unitPrefab, transform.position, Quaternion.identity, playerUnits);

        }
    }
}
