using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fabricator.Units;

public class UnitSpawner : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnUnit();
        }
    }

    private void SpawnUnit()
    {
        GameObject spawnedUnit = Instantiate(unitPrefab, unitSpawnPoint.position, Quaternion.identity);
        TestUnit newUnit = spawnedUnit.GetComponent<TestUnit>();
        newUnit.Move(unitSpawnPoint.position + new Vector3(0, 0, 5));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print("lol");
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        print("lol2");
        SpawnUnit();
    }
}
