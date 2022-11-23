using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fabricator.Units;

public class UnitSpawner : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;

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
        // Decide spawn position
        Vector3 spawnPoint;
        if (!isEnemy)
            spawnPoint = unitSpawnPoint.position;
        else
            spawnPoint = new Vector3(0, 0, 12);

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

        // Tell ally unit to move to rally point
        if (!isEnemy)
            newUnit.Move(spawnPoint + new Vector3(0, 0, 5));
        else
        // Set enemy unit as an enemy
        {
            newUnit.isEnemy = true;
            spawnedUnit.layer = 7;
        }
    }
}
