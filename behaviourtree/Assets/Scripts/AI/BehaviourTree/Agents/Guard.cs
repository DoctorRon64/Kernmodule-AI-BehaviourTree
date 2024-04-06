﻿using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float keepPatrolDistance = 1f;
    [SerializeField] private float keepWeaponDistance = 1f;
    [SerializeField] private float keepPlayerDistance = 1f;
    [SerializeField] private float weaponDetectRange = 15f;
    [SerializeField] private float playerDetectRange = 5f;
    public int MaxHealth { get; } = 100;
    public int Health { get; set; }
    public Weapon Weapon = null;

    private Transform[] wayPoints;
    private BTBaseNode tree;
    private NavMeshAgent agent;

    private void Awake()
    {
        Health = MaxHealth;
        agent = GetComponent<NavMeshAgent>();
        wayPoints = FindObjectsOfType<WayPoint>().Select(_waypoint => _waypoint.transform).ToArray();
    }

    private void Start()
    {
        Blackboard blackboard = new Blackboard();
        blackboard.SetVariable(VariableNames.EnemyHealth, 100);
        blackboard.SetVariable(VariableNames.TargetPatrolPosition, new Vector3(0, 0, 0));
        blackboard.SetVariable(VariableNames.CurrentPatrolIndex, -1);

        Player player = FindObjectOfType<Player>();
        blackboard.SetVariable(VariableNames.TargetPlayer, player.gameObject);
        Debug.Log(blackboard.GetVariable<GameObject>(VariableNames.TargetPlayer));

        tree = new BTRepeater(wayPoints.Length,
            new BTSelector(
                new BTConditional(
                    // Check if player is close
                    new BTIsPlayerInHood(playerDetectRange, transform.position),
                    new BTSelector(
                        new BTConditional(
                            // Check if guard has a weapon
                            new BTGuardHasWeapon(Weapon, this),
                            // Guard has a weapon, move to player and attack
                            new BTRepeater(
                                -1,
                                new BTSequence(
                                    new BTMoveToPlayer(agent, moveSpeed, keepPlayerDistance),
                                    new BTAttackPlayer()
                                )
                            )
                        ),
                        new BTSequence(
                            // Guard does not have a weapon, find closest weapon and move to it
                            new BTGetClosestWeaponPos(agent, weaponDetectRange),
                            new BTMoveToWeapon(agent, moveSpeed, keepWeaponDistance)
                        )
                    )
                ),
                new BTSequence(
                    // Patrol behavior
                    new BTGetNextPatrolPosition(wayPoints),
                    new BTMoveToPatrolPoint(agent, moveSpeed, keepPatrolDistance)
                )
            )
        );

        tree.SetupBlackboard(blackboard);
    }

    private void FixedUpdate()
    {
        TaskStatus result = tree.Tick();
    }

    public void TakeDamage(int _damage)
    {
        Health -= _damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (!_other.TryGetComponent(out IPickupable pickup)) return;
        pickup.Pickup();

        if (pickup is not Weapon weapon) return;
        Weapon = weapon;
    }
}