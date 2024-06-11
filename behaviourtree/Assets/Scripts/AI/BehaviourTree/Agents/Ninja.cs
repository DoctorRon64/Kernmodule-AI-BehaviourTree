using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Ninja : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float playerDetectRange = 1f;
    [SerializeField] private float playerKeepDistance = 1f;
    [SerializeField] private float coverKeepDistance = 0.4f;
    [SerializeField] private float maxFollowDistance = 3f;
    [SerializeField] private GameObject smokeBombPrefab;
    [SerializeField] private Transform throwPoint;

    private Transform[] coverPoints;
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Blackboard blackboard;

    private Transform currentAttacker = null;
    private bool isPlayerBeingAttacked = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        coverPoints = FindObjectsOfType<CoverPoint>().Select(_coverPoints => _coverPoints.transform).ToArray();
        EventManager.AddListener<Transform>(EventType.AttackerTarget, SetAttacker);
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
        EventType ninjaText = EventType.NinjaText;

        tree = new BTRepeater(coverPoints.Length,
            new BTSelector(
                new BTConditional(
                    () => currentAttacker != null,
                    new BTSequence(
                        new BTFindCover(coverPoints, transform),
                        new BTMoveToCover(agent, ninjaText, moveSpeed, coverKeepDistance),
                        new BTThrowSmokeBomb(smokeBombPrefab, throwPoint, currentAttacker)
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

    private void PlayerBeingAttacked()
    {
        isPlayerBeingAttacked = true;
        Debug.Log("Player Attacking = " + isPlayerBeingAttacked);
    }

    private bool IsPlayerInHood(float _range, Vector3 _position)
    {
        GameObject player = blackboard.GetVariable<GameObject>(VariableNames.TargetPlayer);
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(_position, player.transform.position);
        return distanceToPlayer <= _range;
    }

    private void SetAttacker(Transform _transform)
    {
        currentAttacker = _transform;
        Debug.Log("Current Attacker = " + currentAttacker);
    }
}