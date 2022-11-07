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
    private float damage;

    void Update()
    {
        if (!move)
            return;

        if (target != null)
        {
            targetPosition = target.transform.position;
            destination = targetPosition - transform.position;
        }
        else
            Destroy(gameObject, 1);

        distance = Vector3.Distance(transform.position, targetPosition);
        if (distance <= 0.5f)
        {
            if (targetUnit != null)
                targetUnit.HP -= damage;

            Destroy(gameObject);
        }

        if (move)
            transform.Translate((destination.normalized) * Time.deltaTime * 30);
    }

    public void StartMoving(Transform unit, float amount)
    {
        damage = amount;
        target = unit;
        targetUnit = unit.GetComponentInParent<TestUnit>();
        move = true;
    }
}
