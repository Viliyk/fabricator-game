using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    NavMeshAgent myAgent;
    public LayerMask ground;

    Vector3 destination;

    bool stopMoving = true;

    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            //mousePos.y = 0f;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                destination = hit.point;
                destination.y = 1f;

                stopMoving = false;
            }
        }

        if (!stopMoving)
        {
            Vector3 heading = destination - transform.position;
            Vector3 direction = heading / heading.magnitude;

            if (heading.sqrMagnitude < 0.01f)
                stopMoving = true;
            else
                transform.position += direction * 5 * Time.deltaTime;
        }
    }
}
