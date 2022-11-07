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
        [SerializeField] private GameObject projectile = null;
        private Camera mainCamera;
        public LayerMask ground;
        public LayerMask unitLayer;
        private GameObject spawnedProjectile;

        public Vector3 destination;
        public bool forceMove = false;

        private bool selected = false;

        public Collider[] rangeColliders;
        public Transform aggroTarget = null;
        private bool hasAggro = false;
        private float distance;

        public Slider healthBar;

        public bool isEnemy;

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

            if (isEnemy)
                unitBase.material.color = Color.red;
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
                        // Shoot projectile
                        spawnedProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
                        spawnedProjectile.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        spawnedProjectile.GetComponent<ProjectileBehaviour>().StartMoving(aggroTarget, AD);
                        // Reset attack cooldown
                        attackCD = 1 / AS;
                    }
                }
            }

            if (HP <= 0)
                Die();

            // Unit commands
            if (!Mouse.current.rightButton.wasPressedThisFrame)
                return;
            if (!selected)
                return;

            // Check what was clicked
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            //if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
            //    return;
            //forceMove = true;
            //Move(hit.point);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                LayerMask layerHit = hit.transform.gameObject.layer;

                switch (layerHit.value)
                {
                    default:
                        break;
                    case 3:     // Ground layer
                        forceMove = true;
                        Move(hit.point);
                        break;
                    case 6:     // Unit layer
                        forceMove = false;
                        break;
                }
            }
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
            if (isEnemy)
                return;

            selected = true;
            unitBase.material.color = Color.green;
        }

        public void Deselect()
        {
            if (isEnemy)
                return;

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
                TestUnit unit = unitInRange.GetComponentInParent<TestUnit>();
                // Ignore null units
                if (unit == null)
                    continue;
                // Ignore allied units
                if (isEnemy == unit.isEnemy)
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
            UnitSelection.Instance.selectedObjects.Remove(GetComponent<ISelectable>());
            Destroy(gameObject);
        }
    }
}
