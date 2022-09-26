using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fabricator.Units;

public class ProjectileBehaviour : MonoBehaviour
{
    private bool move = false;
    public float distance;
    private Transform target;
    private TestUnit targetUnit;
    public Vector3 targetPosition;
    public Vector3 destination;

    void Update()
    {
        if (!move)
            return;

        if (target != null)
        {
            targetPosition = target.transform.position;
            destination = targetPosition - transform.position;
        }

        distance = Vector3.Distance(transform.position, targetPosition);
        if (distance <= 0.1f)
        {
            targetUnit.HP -= 1;
            Destroy(gameObject);
        }

        if (move)
            transform.Translate((destination.normalized) * Time.deltaTime * 30);
    }

    public void StartMoving(Transform unit)
    {
        target = unit;
        targetUnit = unit.GetComponentInParent<TestUnit>();
        move = true;
    }
}
