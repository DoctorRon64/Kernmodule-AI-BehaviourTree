using UnityEngine;
using UnityEngine.AI;

public class Ninja : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float playerDetectRange = 1f;
    [SerializeField] private float maxFollowDistance = 3f;
    
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Blackboard blackboard;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        this.SetupBlackboard();
    }
    
    private void SetupBlackboard()
    {
        blackboard = new Blackboard();

        Player player = FindObjectOfType<Player>();
        blackboard.SetVariable(VariableNames.TargetPlayer, player.gameObject);
    }

    private void Start()
    {
        Player player = FindObjectOfType<Player>();
        blackboard.SetVariable(VariableNames.TargetPlayer, player.gameObject);
        
        /*tree = new BTRepeater(-1,
            new BTSelector(
                new BTConditional(
                    new BTIsPlayerInHood(playerDetectRange, transform.position),
                    new BTSequence(
                        // Follow the player within a certain distance
                        new BTMoveToPlayer(agent, moveSpeed, maxFollowDistance)
                    )
                ),
                new BTConditional(
                    // Check if the player is being attacked
                    new BTIsPlayerBeingAttacked(),
                    new BTSequence(
                        // Search for cover to hide behind
                        *//*new BTFindCover(agent, coverPoints),
                        // After finding cover, throw bombs at the guard
                        new BTThrowBombs(agent, guardTransform, bombPrefab)*//*
                    )
                )
            )
        );*/
    }

    private void FixedUpdate()
    {
        tree?.Tick();
    }
}
