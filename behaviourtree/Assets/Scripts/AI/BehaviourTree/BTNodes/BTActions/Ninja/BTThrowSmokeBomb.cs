using System;
using UnityEngine;
using UnityEngine.AI;

public class BTThrowSmokeBomb : BTBaseNode
{
    private readonly BombController bombController;
    private readonly Transform throwPoint;
    private readonly NavMeshAgent agent;
    private Transform enemyTransform;

    public BTThrowSmokeBomb(NavMeshAgent _agent ,BombController _bombController, Transform _throwPoint)
    {
        bombController = _bombController;
        throwPoint = _throwPoint;
        agent = _agent;
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        EventManager.InvokeEvent(EventType.NinjaText, GetType().Name);
        enemyTransform = blackboard.GetVariable<Transform>(VariableNames.TargetEnemy);
    }

    protected override TaskStatus OnUpdate()
    {
        if (enemyTransform == null) 
        {
            Debug.LogWarning("No attacker found!" + enemyTransform);
            return TaskStatus.Failed;
        }

        Vector2 direction = (enemyTransform.position - throwPoint.position).normalized;
        bombController.ThrowBomb(direction, throwPoint);
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        agent.transform.rotation = rotation;

        return TaskStatus.Success;
    }
}

public class BombController
{
    private readonly float throwForce;
    private readonly float fireRate;
    private readonly ObjectPool<Bomb> bombPool;
    
    private bool isShooting;
    private float nextFireTime;

    public BombController(GameObject _bombPrefab, int _amount, float _throwForce, float _fireRate)
    {
        throwForce = _throwForce;
        fireRate = _fireRate;
        if (_bombPrefab == null ) return;
        
        bombPool = new ObjectPool<Bomb>(_bombPrefab.GetComponent<Bomb>());
        for (int i = 0; i < _amount; i++)
        {
            Bomb bomb = (Bomb)bombPool.AddNewItemToPool();
            bomb.SetupBomb(bombPool);
        }
    }

    public void ThrowBomb(Vector2 _direction, Transform _shootingPoint)
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireRate;
        Bomb bomb = (Bomb)bombPool.RequestObject(_shootingPoint.position);
        
        if (bomb == null) return;
        bomb.SetPosition(_shootingPoint.position);
        bomb.SetThrowDirection(_direction, throwForce);
        bomb.SetRotation(_direction);
    }
}