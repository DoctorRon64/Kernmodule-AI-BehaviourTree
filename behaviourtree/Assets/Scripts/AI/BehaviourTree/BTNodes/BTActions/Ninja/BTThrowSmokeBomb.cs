using UnityEngine;

public class BTThrowSmokeBomb : BTBaseNode
{
    private readonly Transform enemyTransform;
    private readonly GameObject smokeBombPrefab;
    private readonly Transform throwPoint;

    public BTThrowSmokeBomb(Transform _enemyTransform, GameObject _smokeBombPrefab, Transform _throwPoint)
    {
        enemyTransform = _enemyTransform;
        smokeBombPrefab = _smokeBombPrefab;
        throwPoint = _throwPoint;
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