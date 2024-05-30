using System;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float rotationSpeed = 2.0f;
    [SerializeField] private int maxHealth = 30;
    public int Health { get; set; }
    
    private Rigidbody2D rb2d;
    private Vector2 moveDirection;
    
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
        //EventManager.InvokeEvent(EventType.OnPlayerAttack);
        Health -= _damage;
        
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        EventManager.InvokeEvent(EventType.OnPlayerDied, true);
        gameObject.SetActive(false);
    }
}