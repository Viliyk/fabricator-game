using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private bool move = false;
    private Vector3 destination;
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.7f);
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;

        if (move)
            transform.Translate((target.transform.position - transform.position) * Time.deltaTime * 5);
    }

    public void StartMoving(Transform unit)
    {
        target = unit;
        move = true;
    }
}
