using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fabricator.Units;

public class EnemySpawner : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;

    private List<TestUnit> unitList = new List<TestUnit>();

    // ***Test***
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnUnit(null, true);
        }
    }

    // ***Test***
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        SpawnUnit(null, true);
    }

    public void SpawnUnit(ThisCard activatedCard, bool isEnemy)
    {
        // Decide spawn position
        Vector3 spawnPoint;
        spawnPoint = unitSpawnPoint.position;

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

        // Set enemy unit as an enemy
        spawnedUnit.layer = 7;
        newUnit.isEnemy = true;
        newUnit.isWaiting = true;
        unitList.Add(newUnit);

        // Tell spawned unit to move to rally point
        newUnit.Move(spawnPoint + new Vector3(0, 0, -55), true);
    }

    public void UnleashEnemies()
    {
        foreach (TestUnit unit in unitList)
        {
            unit.isWaiting = false;
        }
        unitList.Clear();
    }
}
