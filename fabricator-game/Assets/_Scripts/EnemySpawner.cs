using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fabricator.Units;

public class EnemySpawner : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;

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

        // Tell spawned unit to move to rally point
        newUnit.Move(spawnPoint + new Vector3(0, 0, -55));

        // Set enemy unit as an enemy
        newUnit.isEnemy = true;
        spawnedUnit.layer = 7;
    }
}
