using UnityEngine;

public class Bomb : MonoBehaviour, IPoolable
{
    private Rigidbody2D rb2d;
    private ObjectPool<Bomb> objectPool;
    public Transform owner;
    public bool Active { get; set; }
    private readonly int damageValue = 1;

    public void SetupBomb(ObjectPool<Bomb> _pool)
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 1; 
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

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damageValue);
            DisablePoolabe();
            objectPool.DeactivateItem(this);
        }

        if (!_other.gameObject.TryGetComponent(out Wall wall)) return;
        DisablePoolabe();
        objectPool.DeactivateItem(this);
    }
    
    public void DisablePoolabe()
    {
        rb2d.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    public void EnablePoolabe()
    {
        rb2d.velocity = Vector2.zero;
        gameObject.SetActive(true);
    }

    public void SetPosition(Vector2 _pos)
    {
        transform.position = _pos;
    }
}