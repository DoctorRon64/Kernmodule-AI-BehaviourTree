using System.Collections;
using UnityEngine;

public class Gun : Item
{
    [Header("Shooting")] 
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int bulletAmount = 10;
    [SerializeField] private float bulletSpawnDistance = 1.0f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.2f;
    private ObjectPool<Bullet> bulletPool;
    private bool isShooting;
    private float nextFireTime;

    private void Awake()
    {
        bulletPool = new ObjectPool<Bullet>(bulletPrefab.GetComponent<Bullet>());
        for (int i = 0; i < bulletAmount; i++)
        {
            Bullet bullet = (Bullet)bulletPool.AddNewItemToPool();
            bullet.SetupBullet(bulletPool);
        }
    }
    
    public void ShootBullet(Vector2 _direction, Transform _shootingPoint)
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireRate;
        Bullet bullet = (Bullet)bulletPool.RequestObject(_shootingPoint.position);
        
        if (bullet == null) return;
        bullet.SetDirection(_direction, bulletSpeed);
        bullet.SetRotation(_direction);
    }
}