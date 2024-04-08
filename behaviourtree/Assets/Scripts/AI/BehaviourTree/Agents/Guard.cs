﻿using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float keepPatrolDistance = 1f;
    [SerializeField] private float keepWeaponDistance = 1f;
    [SerializeField] private float keepPlayerDistance = 1f;
    [SerializeField] private float weaponDetectRange = 15f;
    [SerializeField] private float playerDetectRange = 5f;

    [SerializeField] private Transform shootingPoint;
    [HideInInspector] public Weapon Weapon = null;

    private Transform[] wayPoints;
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Blackboard blackboard;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        wayPoints = FindObjectsOfType<WayPoint>().Select(_waypoint => _waypoint.transform).ToArray();
        this.SetupBlackboard();
    }

    private void SetupBlackboard()
    {
        blackboard = new Blackboard();
        blackboard.SetVariable(VariableNames.EnemyHealth, 100);
        blackboard.SetVariable(VariableNames.TargetPatrolPosition, new Vector3(0, 0, 0));
        blackboard.SetVariable(VariableNames.CurrentPatrolIndex, -1);

        Player player = FindObjectOfType<Player>();
        blackboard.SetVariable(VariableNames.TargetPlayer, player.gameObject);
    }

    private void Start()
    {
        Vector3 agentPosition = transform.position;
        tree = new BTRepeater(wayPoints.Length,
            new BTSelector(
                new BTSequence(
                    // Check if guard does not have a weapon and player is close
                    new BTConditional(
                        new BTIsPlayerInHood(playerDetectRange, agentPosition),
                        new BTGuardHasNoWeapon(Weapon, this)
                    ),
                    // Guard does not have a weapon and player is close, go to the weapon
                    new BTGetClosestWeaponPos(agent, weaponDetectRange),
                    new BTMoveToWeapon(agent, moveSpeed, keepWeaponDistance)
                ),
                new BTConditional(
                    // Check if player is close
                    new BTIsPlayerInHood(playerDetectRange, agentPosition),
                    new BTSelector(
                        new BTConditional(
                            // Check if guard has a weapon
                            new BTGuardHasWeapon(Weapon, this),
                            // Guard has a weapon, move to player and attack
                            new BTSequence(
                                new BTMoveToPlayer(agent, moveSpeed, keepPlayerDistance),
                                new BTAttackPlayer((Gun)Weapon, this, shootingPoint)
                            )
                        ),
                        new BTSequence(
                            // Guard does not have a weapon, find closest weapon and move to it
                            new BTGetClosestWeaponPos(agent, weaponDetectRange),
                            new BTMoveToWeapon(agent, moveSpeed, keepWeaponDistance),
                            new BTMoveToPlayer(agent, moveSpeed, keepPlayerDistance)
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

    private void OnDisable()
    {
        //anders word valentijn boos
        EventManager.RemoveAllListeners();
    }

    private void FixedUpdate()
    {
        TaskStatus result = tree.Tick();
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (!_other.TryGetComponent(out IPickupable pickup)) return;
        pickup.Pickup();

        if (pickup is not Weapon weapon) return;
        Weapon = weapon;
    }
}