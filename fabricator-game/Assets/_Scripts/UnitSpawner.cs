using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fabricator.Units;

public class UnitSpawner : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;

    Vector3 spawnPoint;
    private List<TestUnit> unitList = new List<TestUnit>();

    void Start()
    {
        // Decide spawn position
        spawnPoint = unitSpawnPoint.position;
    }

    // ***Test***
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnUnit(null, false);
        }
    }

    // ***Test***
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        SpawnUnit(null, false);
    }

    public void SpawnUnit(ThisCard activatedCard, bool isEnemy)
    {
        // Spawn unit
        GameObject spawnedUnit = Instantiate(unitPrefab, spawnPoint, Quaternion.identity);
        TestUnit newUnit = spawnedUnit.GetComponent<TestUnit>();
        CanTakeDamage unitHealth = spawnedUnit.GetComponent<CanTakeDamage>();

        // Set stats
        if (activatedCard != null)
        {
            newUnit.AD = activatedCard.attack;
            unitHealth.HP = activatedCard.health;
        }

        unitList.Add(newUnit);

        // Tell spawned unit to move to rally point
        newUnit.Move(spawnPoint + new Vector3(5, 0, 5), true);
    }

    public void UnleashUnits()
    {
        foreach (TestUnit unit in unitList)
        {
            unit.Move(spawnPoint + new Vector3(55, 0, 55), true);
        }
        unitList.Clear();
    }
}
