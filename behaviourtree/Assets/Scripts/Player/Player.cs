using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float rotationSpeed = 2.0f;
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private bool invulnerable = false;

    [SerializeField] private float bulletDetectionRadius = 3f;
    [SerializeField] private float isBeingAttackedCooldown = 2.5f;

    private int health;
    public int Health
    {
        get => health;
        set
        {
            if (health == value) return;
            health = value;
            EventManager.InvokeEvent(EventType.OnPlayerHpChanged, value);
        }
    }
    
    private Rigidbody2D rb2d;
    private Vector2 moveDirection;
    private Coroutine damageCoroutine;
    
    private void Awake()
    {
        Health = maxHealth;
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        var verticalInput = Input.GetAxis("Vertical");
        var horizontalInput = Input.GetAxis("Horizontal");
        moveDirection = new Vector2(horizontalInput, verticalInput).normalized;
        
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        CheckForBullets();
        
        if (moveDirection != Vector2.zero)
        {
            // Move
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
            rb2d.velocity = moveDirection * currentSpeed;
        }
        else
        {
            rb2d.velocity = Vector2.zero;
        }
    }
    
    public void TakeDamage(int _damage)
    {
        if (invulnerable) return;
        Health -= _damage;
        
        if (Health <= 0)
        {
            Die();
        }
    }
    
    private IEnumerator DamageCoroutine()
    {
        yield return new WaitForSeconds(isBeingAttackedCooldown);
        EventManager.InvokeEvent(EventType.OnPlayerAttack, false);
    }

    private void Die()
    {
        EventManager.InvokeEvent(EventType.OnPlayerDied, true);
        Destroy(gameObject);
    }

    private void CheckForBullets()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, bulletDetectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.TryGetComponent(out Bullet _bullet)) continue;
            EventManager.InvokeEvent(EventType.OnPlayerAttack, true);
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
            }

            damageCoroutine = StartCoroutine(DamageCoroutine());
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the detection radius for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bulletDetectionRadius);
    }
}