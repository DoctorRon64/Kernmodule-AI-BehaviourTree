﻿using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour, IPoolable
{
    private Rigidbody2D rb2d;
    private ObjectPool<Bomb> objectPool;
    public bool Active { get; set; }
    private Coroutine disableCoroutine;

    public void SetupBomb(ObjectPool<Bomb> _pool)
    {
        rb2d = GetComponent<Rigidbody2D>();
        objectPool = _pool;
    }
    
    public void SetThrowDirection(Vector2 _direction, float _force)
    {
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(_direction.normalized * _force, ForceMode2D.Impulse);
    }
    public void SetRotation(Vector2 _direction)
    {
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    public void SetPosition(Vector2 _pos)
    {
        transform.position = _pos;
    }

    private void OnCollisionEnter2D(Collision2D _other)
    {
        if (_other.gameObject.TryGetComponent(out IStunnable stunnable))
        {
            stunnable.Stun();
            objectPool.DeactivateItem(this);
            DisablePoolabe();
        }

        if (!_other.gameObject.TryGetComponent(out Wall wall)) return;
        objectPool.DeactivateItem(this);
        DisablePoolabe();
    }
    
    public void DisablePoolabe()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }
        
        rb2d.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    public void EnablePoolabe()
    {
        rb2d.velocity = Vector2.zero;
        gameObject.SetActive(true);
        
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(DisableAfterTime(4f));
    }
    
    private IEnumerator DisableAfterTime(float _time)
    {
        yield return new WaitForSeconds(_time);
        objectPool.DeactivateItem(this);
        DisablePoolabe();
    }
}