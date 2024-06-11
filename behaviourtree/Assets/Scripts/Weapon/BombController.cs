using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab; // Changed variable name for clarity
    [SerializeField] private int bombAmount = 10;
    [SerializeField] private float bombSpawnDistance = 1.0f;
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float fireRate = 0.2f;

    private ObjectPool<Bomb> bombPool;
    private bool isShooting;
    private float nextFireTime;

    private void Awake()
    {
        bombPool = new ObjectPool<Bomb>(bombPrefab.GetComponent<Bomb>());
        for (int i = 0; i < bombAmount; i++)
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