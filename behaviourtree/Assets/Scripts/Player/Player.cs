using System;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float sprintSpeed = 6;
    [SerializeField] private float health = 100;
    
    private Rigidbody2D rb2d;
    private Animator animator;
    private Vector2 moveDirection;

    public Action onPlayerDeath;
    
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
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
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            rb2d.velocity = moveDirection * currentSpeed;
            
            ChangeAnimation("Walk", 0.05f);
        }
        else
        {
            rb2d.velocity = Vector2.zero;
            ChangeAnimation("Idle", 0.05f);
        }
    }
    
    private void ChangeAnimation(string animationName, float fadeTime)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) && !animator.IsInTransition(0))
        {
            animator.CrossFade(animationName, fadeTime);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        onPlayerDeath.Invoke();
        gameObject.SetActive(false);
    }
}