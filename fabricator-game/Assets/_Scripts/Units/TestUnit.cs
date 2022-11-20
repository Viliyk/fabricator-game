using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace Fabricator.Units
{
    public class TestUnit : MonoBehaviour, ISelectable
    {
        [SerializeField] private NavMeshAgent myAgent = null;
        [SerializeField] private MeshRenderer unitBase = null;
        [SerializeField] private GameObject projectile = null;
        [SerializeField] private TMP_Text powerText = null;
        [SerializeField] private SimpleFlash flashEffect = null;
        private Camera mainCamera;
        public LayerMask ground;
        public LayerMask enemyLayer;
        private GameObject spawnedProjectile;

        public Vector3 destination;
        public bool forceMove = false;
        public bool forceAttack = false;
        private bool isRecovering = false;

        private bool selected = false;

        public Collider[] rangeColliders;
        public Transform aggroTarget = null;
        private bool hasAggro = false;
        private float distance;

        public Slider healthBar;

        public bool isEnemy;

        private float aggroRange = 10;
        private float range = 6;
        public float HP = 500;
        public float AD = 1;
        private float AS = 1;

        private float attackCD = 0;
        private float windup = 0.2f;
        private float recovery = 0.5f;

        void Start()
        {
            mainCamera = Camera.main;
            healthBar.maxValue = HP;

            if (isEnemy)
            {
                unitBase.material.color = Color.red;
                enemyLayer = LayerMask.NameToLayer("Allies");
            }
            else
                enemyLayer = LayerMask.NameToLayer("Enemies");

            if (isEnemy)
            {
                myAgent.speed = 3;
                range = 2;
            }
        }

        void Update()
        {
            // Update UI elements
            powerText.text = "" + AD;
            healthBar.value = HP;

            // Tick down attack cooldown
            if (attackCD > 0)
                attackCD -= Time.deltaTime;

            // Remove attack command if move command is issued or target is null
            if (forceMove || aggroTarget == null)
                forceAttack = false;

            // Remove move command when arriving at destination
            if (Vector3.Distance(transform.position, destination) <= 0.1f)
                forceMove = false;

            // Look for targets when commands are not issued
            if (!forceAttack && !forceMove)
                CheckForTargets();

            // Chase and attack target
            if (aggroTarget != null && !forceMove)
            {
                // Move to range of aggro target
                if (Vector3.Distance(transform.position, aggroTarget.position) > range)
                    Move(aggroTarget.position);
                else
                {
                    // Stop when in range
                    //Move(transform.position);
                    myAgent.SetDestination(transform.position);
                    // Attack
                    if (attackCD <= 0)
                    {
                        StopCoroutine(Attack());
                        StartCoroutine(Attack());
                    }
                }
            }

            // Input check
            if (Mouse.current.rightButton.wasPressedThisFrame && selected)
                UnitCommands(false);    // "Right-click" for normal command
            if (Keyboard.current.aKey.wasPressedThisFrame && selected)
                UnitCommands(true);     // "A" for attack-move
            if (Keyboard.current.tabKey.wasPressedThisFrame && selected)
                UnitCommands(true);     // "Tab" for attack-move

            // Destroy this unit when HP reaches 0
            if (HP <= 0)
                Die();
        }

        private void UnitCommands(bool IsAttackMove)
        {
            // Check what was clicked
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                LayerMask layerHit = hit.transform.gameObject.layer;

                switch (layerHit.value)
                {
                    default:
                        break;
                    case 3:     // Ground layer: Move to this position
                    case 6:     // Ally layer
                        if (!IsAttackMove)
                            forceMove = true;
                        else
                            forceMove = false;
                        Move(hit.point);
                        break;
                    case 7:     // Enemy layer: Choose this enemy as a target
                        forceMove = false;
                        forceAttack = true;
                        aggroTarget = hit.transform;
                        break;
                }
            }
        }

        public void Move(Vector3 position)
        {
            if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                return;

            if (!isRecovering || forceMove)
            {
                destination = hit.position;
                myAgent.SetDestination(destination);
            }
        }

        IEnumerator Attack()
        {
            isRecovering = true;
            // Reset attack cooldown
            attackCD = 1 / AS;
            yield return new WaitForSeconds(windup);
            // Shoot projectile
            spawnedProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
            spawnedProjectile.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            spawnedProjectile.GetComponent<ProjectileBehaviour>().StartMoving(aggroTarget, AD);

            StopCoroutine(StartRecovery());
            StartCoroutine(StartRecovery());
        }

        IEnumerator StartRecovery()
        {
            isRecovering = true;
            yield return new WaitForSeconds(recovery);
            isRecovering = false;
        }

        private void CheckForTargets()
        {
            //Vector3 y = new Vector3(0, 1, 0);
            //if (aggroTarget != null)
            //    DrawArrow.ForDebug(transform.position + y, aggroTarget.position - transform.position, Color.red);
            float radius = aggroRange;
            float distanceToTarget;

            //if (aggroTarget != null)
            //{
            //    distanceToTarget = Vector3.Distance(transform.position, aggroTarget.position);

            //    if (distanceToTarget < aggroRange)
            //        radius = distanceToTarget;
            //}

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
                if (unitInRange.gameObject.layer == enemyLayer)
                {
                    if (closest == null)
                        closest = unitInRange;
                    else if (Vector3.Distance(transform.position, unitInRange.position) < Vector3.Distance(transform.position, closest.position))
                        closest = unitInRange;
                }
            }
            aggroTarget = closest;
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

        public void TakeDamage(float amount)
        {
            HP -= amount;

            if (flashEffect != null && HP > 0)
                flashEffect.Flash();
        }

        private void Die()
        {
            UnitSelection.Instance.selectedObjects.Remove(GetComponent<ISelectable>());
            Destroy(gameObject);
        }
    }
}
