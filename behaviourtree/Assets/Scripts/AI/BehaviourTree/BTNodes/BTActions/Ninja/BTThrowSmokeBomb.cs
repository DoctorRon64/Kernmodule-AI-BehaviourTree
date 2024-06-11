using UnityEngine;

public class BTThrowSmokeBomb : BTBaseNode
{
    private readonly GameObject smokeBombPrefab;
    private readonly Transform throwPoint;
    private Transform enemyTransform;

    public BTThrowSmokeBomb(GameObject _smokeBombPrefab, Transform _throwPoint, Transform _enemyTransform)
    {
        smokeBombPrefab = _smokeBombPrefab;
        throwPoint = _throwPoint;
        enemyTransform = _enemyTransform;
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        EventManager.InvokeEvent(EventType.NinjaText, GetType().Name);
    }

    protected override TaskStatus OnUpdate()
    {
        if (enemyTransform == null) { Debug.LogWarning("no attacker found!"); return TaskStatus.Failed; }
        
        Vector3 position = throwPoint.position;
        GameObject smokeBomb = Object.Instantiate(smokeBombPrefab, position, Quaternion.identity);
        smokeBomb.GetComponent<Rigidbody>().AddForce((enemyTransform.position - position).normalized * 10f, ForceMode.VelocityChange);

        return TaskStatus.Success;
    }
}