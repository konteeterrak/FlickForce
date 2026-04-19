using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 50f;
    public float currentHealth;
    public float damage = 10f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float wallEmergeDuration = 1f;

    [Header("Attack Settings")]
    public float attackCooldown = 1f;
    private float lastAttackTime;

    [Header("Knockback")]
    public float knockbackForce = 5f;

    [Header("Spawn Animation")]
    public Vector3 spawnOffset = new Vector3(0, 0, 2f);
    private Vector3 targetPosition;
    private bool isEmerging = true;
    private float emergeTimer;

    // เพิ่มตรงนี้
    [Header("Animation")]
    public Animator animator;

    private Transform player;
    private CharacterController controller;
    private Rigidbody rb;
    private Renderer enemyRenderer;
    private Vector3 moveDirection;
    private bool isDead = false;

    void Start()
    {
        if (animator != null)
        {
            animator.applyRootMotion = false;
        }
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        enemyRenderer = GetComponent<Renderer>();

        // หา Animator อัตโนมัติถ้าไม่ได้ Assign
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.useGravity = true;

            if (controller != null)
            {
                Destroy(controller);
                controller = null;
            }
        }
        else if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.radius = 0.5f;
            controller.height = 2f;
        }

        targetPosition = transform.position;
        transform.position = targetPosition - spawnOffset;
        emergeTimer = 0;
    }

    void Update()
    {
        if (isDead) return;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            return;
        }
        if (isEmerging)
        {
            EmergeFromWall();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0;

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            if (distanceToPlayer <= attackRange)
            {
            // หยุดเดิน เล่น Attack
                SetAnimation(false, true);
                AttackPlayer();
            }
            else
            {
            // เดินหาผู้เล่น
                SetAnimation(true, false);
                MoveTowardsPlayer(directionToPlayer);
            }
        }
        else
        {
        // อยู่นอกระยะ → Idle
            SetAnimation(false, false);
        }

        if (controller != null && !controller.isGrounded)
        {
            moveDirection.y -= 9.81f * Time.deltaTime;
        }
    }

    // ฟังก์ชันเซต Animation
    void SetAnimation(bool walking, bool attacking)
    {
        if (animator == null) return;
        animator.SetBool("isWalking", walking);
        animator.SetBool("isAttacking", attacking);
    }

    void EmergeFromWall()
    {
        emergeTimer += Time.deltaTime;
        float progress = emergeTimer / wallEmergeDuration;

        transform.position = Vector3.Lerp(
            targetPosition - spawnOffset,
            targetPosition,
            progress
        );

        if (progress >= 1f)
            isEmerging = false;
    }

    void MoveTowardsPlayer(Vector3 direction)
    {
        if (controller != null)
        {
            moveDirection = direction * moveSpeed;
            moveDirection.y = controller.isGrounded ? 0 : moveDirection.y;
            controller.Move(moveDirection * Time.deltaTime);
        }
        else if (rb != null)
        {
            Vector3 velocity = direction * moveSpeed;
            velocity.y = rb.velocity.y;
            rb.velocity = velocity;
        }
    }

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Vector3 knockbackDir = (player.position - transform.position).normalized;
                knockbackDir.y = 0.5f;
                playerHealth.TakeDamage(damage, knockbackDir * knockbackForce);
            }
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator HitFlash()
    {
        if (enemyRenderer)
        {
            Color originalColor = enemyRenderer.material.color;
            enemyRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            enemyRenderer.material.color = originalColor;
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Enemy died!");

        // เล่น Animation ตาย
        if (animator != null)
            animator.SetBool("isDead", true);

        // รอ Animation ตายเสร็จแล้วค่อย Destroy
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}