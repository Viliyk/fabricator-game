using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        public bool forceMove = false;

        private bool selected = false;

        public Collider[] rangeColliders;
        public Transform aggroTarget = null;
        private bool hasAggro = false;
        private float distance;

        public Slider healthBar;

        private float aggroRange = 10;
        private float range = 5;
        public float HP = 500;
        private float AS = 1;
        private float AD = 1;

        private float attackCD = 0;

        void Start()
        {
            mainCamera = Camera.main;
            unitLayer = LayerMask.NameToLayer("Units");
            healthBar.maxValue = HP;
        }

        void Update()
        {
            healthBar.value = HP;

            if (attackCD > 0)
                attackCD -= Time.deltaTime;

            CheckForTargets();

            if (Vector3.Distance(transform.position, destination) <= 0.1f)
                forceMove = false;

            if (aggroTarget != null && !forceMove)
            {
                // Move to range of aggro target
                if (Vector3.Distance(transform.position, aggroTarget.position) > range)
                    Move(aggroTarget.position);
                else
                {
                    // Stop when in range
                    Move(transform.position);
                    // Attack
                    if (attackCD <= 0)
                    {
                        aggroTarget.parent.gameObject.GetComponent<TestUnit>().HP -= AD;
                        attackCD = 1 / AS;
                    }
                }
            }


            Die();

            // Move command
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
            //Vector3 y = new Vector3(0, 1, 0);
            //if (aggroTarget != null)
            //    DrawArrow.ForDebug(transform.position + y, aggroTarget.position - transform.position, Color.red);
            float radius = aggroRange;
            float distanceToTarget;

            if (aggroTarget != null)
            {
                distanceToTarget = Vector3.Distance(transform.position, aggroTarget.position);

                if (distanceToTarget < aggroRange)
                    radius = distanceToTarget;
            }

            rangeColliders = Physics.OverlapSphere(transform.position, radius);

            if (rangeColliders.Length == 2)
            {
                aggroTarget = null;
                return;
            }

            Transform closest = null;
            for (int i = 0; i < rangeColliders.Length; i++)
            {
                Transform unitInRange = rangeColliders[i].transform;

                // Ignore this unit
                if (unitInRange.parent == transform)
                    continue;
                // Pick closest unit
                if (unitInRange.gameObject.layer == unitLayer)
                {
                    if (closest == null)
                        closest = unitInRange;
                    else if (Vector3.Distance(transform.position, unitInRange.position) < Vector3.Distance(transform.position, closest.position))
                        closest = unitInRange;
                }
            }
            aggroTarget = closest;
        }

        private void Die()
        {
            if (HP <= 0)
            {
                UnitSelection.Instance.selectedObjects.Remove(GetComponent<ISelectable>());
                Destroy(gameObject);
            }
        }
    }
}
