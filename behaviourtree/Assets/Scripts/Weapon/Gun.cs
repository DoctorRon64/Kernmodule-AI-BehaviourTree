using System.Collections;
using UnityEngine;

public class Gun : Weapon
{
    [Header("Shooting")] 
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int bulletAmount = 10;
    [SerializeField] private float bulletSpawnDistance = 1.0f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.2f;
    
    private Transform shootingPoint;
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
        shootingPoint = _shootingPoint;
        //shootingPoint.localPosition = new Vector3(bulletSpawnDistance, 0f, 0f);
        
        if (Time.time < nextFireTime)
            return;
        
        nextFireTime = Time.time + fireRate;
        
        Bullet bullet = (Bullet)bulletPool.RequestObject(shootingPoint.position);
        if (bullet == null) return;
        bullet.SetDirection(_direction, bulletSpeed);
        bullet.SetRotation(_direction);
    }
}