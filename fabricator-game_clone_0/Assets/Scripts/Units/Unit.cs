using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Unit : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent myAgent = null;
    private Camera mainCamera;
    public LayerMask ground;

    Vector3 destination;

    bool stopMoving = true;

    #region Server

    [Command]
    private void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            return;

        myAgent.SetDestination(hit.position);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        mainCamera = Camera.main;
        //myAgent = GetComponent<NavMeshAgent>();
        //destination = transform.position;
    }

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority)
            return;

        if (!Mouse.current.rightButton.wasPressedThisFrame)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            return;

        CmdMove(hit.point);

        //destination = hit.point;
        //destination.y = 1f;

        //stopMoving = false;


        //if (!stopMoving)
        //{
        //    Vector3 heading = destination - transform.position;
        //    Vector3 direction = heading / heading.magnitude;

        //    if (heading.sqrMagnitude < 0.01f)
        //        stopMoving = true;
        //    else
        //        transform.position += direction * 5 * Time.deltaTime;

        //    transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        //}
    }

    #endregion
}
