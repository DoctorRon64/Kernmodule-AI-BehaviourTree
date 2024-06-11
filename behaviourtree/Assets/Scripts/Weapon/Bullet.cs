using System;
using UnityEngine;
public class Bullet : MonoBehaviour, IPoolable
{
	private Rigidbody2D rb;
	private ObjectPool<Bullet> objectPool;
	
	public bool Active { get; set; }
	private readonly int damageValue = 1;
	
	public void SetupBullet(ObjectPool<Bullet> _pool)
	{
		rb = GetComponent<Rigidbody2D>();
		objectPool = _pool;
	}
	
    public void SetDirection(Vector2 _direction, float _speed)
	{
		rb.velocity = _direction.normalized * _speed;
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

	public void SetPosition(Vector2 _position)
	{
		transform.position = _position;
	}

	public void DisablePoolabe()
	{
		rb.velocity = Vector2.zero;
		gameObject.SetActive(false);
	}

	public void EnablePoolabe()
	{
		SetDirection(Vector2.left, 10f);
		gameObject.SetActive(true);
	}
}
