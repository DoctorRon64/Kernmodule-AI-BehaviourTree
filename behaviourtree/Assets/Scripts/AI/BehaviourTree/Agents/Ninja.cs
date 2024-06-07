using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Ninja : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float playerDetectRange = 1f;
    [SerializeField] private float playerKeepDistance = 1f;
    [SerializeField] private float maxFollowDistance = 3f;
    [SerializeField] private GameObject smokeBombPrefab;
    [SerializeField] private Transform throwPoint;
    
    private Transform[] coverPoints;
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Blackboard blackboard;

    private bool isPlayerBeingAttacked = false;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        coverPoints = FindObjectsOfType<WayPoint>().Select(_coverPoints => _coverPoints.transform).ToArray();
        
        Player player = FindObjectOfType<Player>();
        blackboard = new Blackboard();
        blackboard.SetVariable(VariableNames.TargetPlayer, player.gameObject);
        
        EventManager.AddListener(EventType.OnPlayerAttacked, PlayerBeingAttacked);
    }

    private void Start()
    {
        tree = new BTRepeater(-1,
            new BTSelector(
                new BTConditional(
                    () => IsPlayerInHood(playerDetectRange, transform.position),
                    new BTSequence(
                        new BTMoveToPlayer(agent, moveSpeed, maxFollowDistance)
                    )
                ),
                new BTConditional(
                    () => isPlayerBeingAttacked,
                    new BTSequence(
                        new BTFindCover(coverPoints, transform),
                        new BTMoveToCover(agent, moveSpeed, playerKeepDistance),
                        new BTThrowSmokeBomb(AttackContext.CurrentAttacker.transform, smokeBombPrefab, throwPoint)
                    )
                )
            )
        );

        tree.SetupBlackboard(blackboard);
    }

    private void FixedUpdate()
    {
        TaskStatus result = tree.Tick();
    }

    private void PlayerBeingAttacked()
    {
        Debug.Log("player is getting Attacked!");
        isPlayerBeingAttacked = !isPlayerBeingAttacked;
    }

    private bool IsPlayerInHood(float _range, Vector3 _position)
    {
        GameObject player = blackboard.GetVariable<GameObject>(VariableNames.TargetPlayer);
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(_position, player.transform.position);
        return distanceToPlayer <= _range;
    }
}

public static class AttackContext
{
    public static GameObject CurrentAttacker { get; set; }
}