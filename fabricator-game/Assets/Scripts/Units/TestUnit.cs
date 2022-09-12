using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Fabricator.Units
{
    public class TestUnit : MonoBehaviour, ISelectable
    {
        [SerializeField] private NavMeshAgent myAgent = null;
        [SerializeField] private MeshRenderer unitBase = null;
        private Camera mainCamera;
        public LayerMask ground;
        public LayerMask unitLayer;

        public Vector3 destination;

        private bool selected = false;

        public Collider[] rangeColliders;
        public Transform aggroTarget;
        private bool hasAggro = false;
        private float distance;

        public bool forceMove = false;

        private float aggroRange = 10;
        private float range = 5;
        private float AS = 1;
        private float AD = 2;

        void Start()
        {
            mainCamera = Camera.main;
            unitLayer = LayerMask.NameToLayer("Units");
        }

        void Update()
        {
            CheckForTargets();

            if (Vector3.Distance(transform.position, destination) <= 0.1f)
                forceMove = false;

            if (aggroTarget != null && !forceMove)
            {
                Move(aggroTarget.position);
            }

            if (!Mouse.current.rightButton.wasPressedThisFrame)
                return;
            if (!selected)
                return;
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
                return;
            forceMove = true;
            Move(hit.point);
        }

        private void Move(Vector3 position)
        {
            if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                return;

            destination = hit.position;
            myAgent.SetDestination(destination);
        }

        public void Select()
        {
            selected = true;
            unitBase.material.color = Color.green;
        }

        public void Deselect()
        {
            selected = false;
            unitBase.material.color = Color.white;
        }

        private void CheckForTargets()
        {
            rangeColliders = Physics.OverlapSphere(transform.position, aggroRange);

            if (rangeColliders.Length == 2)
            {
                aggroTarget = null;
                return;
            }

            for (int i = 0; i < rangeColliders.Length; i++)
            {
                Transform unitInRange = rangeColliders[i].transform;
                if (unitInRange.parent == transform)
                    continue;

                if (unitInRange.gameObject.layer == unitLayer)
                {
                    aggroTarget = unitInRange;
                    break;
                }
            }
        }
    }
}
