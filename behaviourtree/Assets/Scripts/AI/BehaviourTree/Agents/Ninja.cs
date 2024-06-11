﻿using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Ninja : MonoBehaviour
{
    [Header("movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float playerDetectRange = 1f;
    [SerializeField] private float coverKeepDistance = 0.4f;
    [SerializeField] private float maxFollowDistance = 3f;
    
    [Header("bombs")]
    [SerializeField] private GameObject smokeBombPrefab;
    [SerializeField] private Transform throwPoint;
    
    [SerializeField] private int bombAmount = 10;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float fireRate = 0.2f;

    private Transform[] coverPoints;
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Blackboard blackboard;

    private bool isPlayerBeingAttacked = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        coverPoints = FindObjectsOfType<CoverPoint>().Select(_coverPoints => _coverPoints.transform).ToArray();
        EventManager.AddListener<Transform>(EventType.AttackerTarget, SetAttacker);
        EventManager.AddListener<bool>(EventType.OnPlayerAttack, PlayerBeingAttacked);
        this.SetupBlackboard();
    }

    private void SetupBlackboard()
    {
        Player player = FindObjectOfType<Player>();
        blackboard = new Blackboard();
        blackboard.SetVariable(VariableNames.TargetPlayer, player.gameObject);
    }

    private void Start()
    {
        BombController bombController = new BombController(smokeBombPrefab, bombAmount, throwForce, fireRate);
        const EventType ninjaText = EventType.NinjaText;

        tree = new BTRepeater(coverPoints.Length,
            new BTSelector(
                new BTConditional(
                    () => isPlayerBeingAttacked,
                    new BTSequence(
                        new BTFindCover(coverPoints, transform),
                        new BTMoveToCover(agent, ninjaText, moveSpeed, coverKeepDistance),
                        new BTThrowSmokeBomb(bombController, throwPoint)
                    )
                ),
                new BTConditional(
                    () => IsPlayerInHood(playerDetectRange, transform.position),
                    new BTSequence(
                        new BTMoveToPlayer(agent, ninjaText, moveSpeed, maxFollowDistance)
                    )
                )
            )
        );

        tree.SetupBlackboard(blackboard);
    }

    private void FixedUpdate()
    {
        if (tree == null)
        {
            Debug.LogError("Behavior tree is not initialized.");
            return;
        }

        TaskStatus result = tree.Tick();
    }

    private void PlayerBeingAttacked(bool _newValue)
    {
        isPlayerBeingAttacked = _newValue;
        Debug.Log("Player Attacking = " + isPlayerBeingAttacked);
    }

    private bool IsPlayerInHood(float _range, Vector3 _position)
    {
        GameObject player = blackboard.GetVariable<GameObject>(VariableNames.TargetPlayer);
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(_position, player.transform.position);
        return distanceToPlayer <= _range;
    }

    private void SetAttacker(Transform _newAttacker)
    {
        Debug.Log("Current Attacker = " + _newAttacker);
        blackboard.SetVariable(VariableNames.TargetEnemy, _newAttacker);
    }
}