using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fabricator.Units
{
    public class UnitHandler : MonoBehaviour
    {
        public static UnitHandler Instance;

        [SerializeField]
        private UnitNew unit, unit2;
        public Transform playerUnits;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            GameObject spawnedUnit = Instantiate(unit.unitPrefab, transform.position, Quaternion.identity, playerUnits);
            GameObject spawnedUnit2 = Instantiate(unit2.unitPrefab, transform.position, Quaternion.identity, playerUnits);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.I))
            {
                Instantiate(unit.unitPrefab, transform.position, Quaternion.identity, playerUnits);
            }

            if (Input.GetKey(KeyCode.O))
            {
                GameObject g;

                g = Instantiate(unit.unitPrefab, new Vector3(0, 0, 10), Quaternion.identity, playerUnits);
                g.GetComponent<TestUnit>().isEnemy = true;
            }
        }
    }
}
