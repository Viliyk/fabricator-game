using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NetworkUnit : NetworkBehaviour, ISelectable
{
    [SerializeField] private NavMeshAgent myAgent = null;
    [SerializeField] private MeshRenderer unitBase = null;
    private Camera mainCamera;
    public LayerMask ground;

    Vector3 destination;

    //bool stopMoving = true;
    private bool selected = false;

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

    void Start()
    {
        if (!hasAuthority)
            unitBase.material.color = Color.red;
    }

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority)
            return;

        if (!Mouse.current.rightButton.wasPressedThisFrame)
            return;

        if (!selected)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
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

    public void Select()
    {
        if (!hasAuthority)
            return;

        selected = true;
        unitBase.material.color = Color.green;
    }

    public void Deselect()
    {
        if (!hasAuthority)
            return;

        selected = false;
        unitBase.material.color = Color.white;
    }

    #endregion
}
