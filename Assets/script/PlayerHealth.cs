using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // เพิ่ม TextMeshPro
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false;
    
    [Header("UI")]
    public Image healthBarFill; // แถบเลือด (Fill)
    public TMP_Text healthText; // เปลี่ยนเป็น TMP_Text
    public GameObject deathPanel; // Panel ตายแล้ว
    
    [Header("Damage Settings")]
    public float damageFlashDuration = 0.1f;
    public Color damageColor = Color.red;
    
    [Header("Fall Damage")]
    public float fallDamageThreshold = 5f; // ความสูงขั้นต่ำที่เริ่มเจ็บ
    public float fallDamageMultiplier = 10f; // ดาเมจต่อความสูง
    
    [Header("Knockback Settings")]
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;

    [Header("Game Over Audio")]
    public AudioClip gameOverSound; // ลาก Audio Clip ใส่ใน Inspector
    
    private CharacterController controller;
    private Camera playerCamera;
    private float lastGroundedHeight;
    private bool isFalling = false;
    private Vector3 knockbackVelocity;
    private float knockbackTimer;
    
    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        
        UpdateHealthUI();
        
        if (deathPanel)
            deathPanel.SetActive(false);
    }
    
    void Update()
    {
        // ตรวจจับการตก
        CheckFallDamage();
        
        // จัดการ Knockback
        HandleKnockback();
    }
    
    // ฟังก์ชันรับดาเมจ
    public void TakeDamage(float damage, Vector3 hitDirection = default)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log($"Player took {damage} damage! Health: {currentHealth}/{maxHealth}");
        
        UpdateHealthUI();
        StartCoroutine(DamageFlash());
        
        // Knockback
        if (hitDirection != default)
        {
            ApplyKnockback(hitDirection);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // Knockback
    public void ApplyKnockback(Vector3 direction)
    {
        knockbackVelocity = direction.normalized * knockbackForce;
        knockbackTimer = knockbackDuration;
    }
    
    void HandleKnockback()
    {
        if (knockbackTimer > 0)
        {
            if (controller != null)
            {
                controller.Move(knockbackVelocity * Time.deltaTime);
            }
            knockbackTimer -= Time.deltaTime;
        }
    }
    
    // ตรวจสอบดาเมจตก
    void CheckFallDamage()
    {
        if (controller == null) return;
        
        if (controller.isGrounded)
        {
            if (isFalling)
            {
                // คำนวณระยะที่ตก
                float fallDistance = lastGroundedHeight - transform.position.y;
                
                if (fallDistance > fallDamageThreshold)
                {
                    float damage = (fallDistance - fallDamageThreshold) * fallDamageMultiplier;
                    TakeDamage(damage);
                    Debug.Log($"Fall Damage: {damage} (fell {fallDistance}m)");
                }
                
                isFalling = false;
            }
            
            lastGroundedHeight = transform.position.y;
        }
        else
        {
            if (!isFalling)
            {
                isFalling = true;
                lastGroundedHeight = transform.position.y;
            }
        }
    }
    
    // ฟังก์ชันฟื้นเลือด
    public void Heal(float amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        UpdateHealthUI();
        Debug.Log($"Player healed {amount}! Health: {currentHealth}/{maxHealth}");
    }
    
    // อัปเดต UI
    void UpdateHealthUI()
    {
        if (healthBarFill)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
            
            // เปลี่ยนสีตามเลือด
            if (currentHealth / maxHealth > 0.5f)
                healthBarFill.color = Color.green;
            else if (currentHealth / maxHealth > 0.25f)
                healthBarFill.color = Color.yellow;
            else
                healthBarFill.color = Color.red;
        }
        
        if (healthText)
        {
            healthText.text = $"{Mathf.RoundToInt(currentHealth)} / {Mathf.RoundToInt(maxHealth)}";
        }
    }
    
    // เอฟเฟกต์กระพริบเมื่อโดนดาเมจ
    System.Collections.IEnumerator DamageFlash()
    {
        if (playerCamera)
        {
            // สร้าง Flash Effect บนหน้าจอ
            GameObject flashObj = new GameObject("DamageFlash");
            flashObj.transform.SetParent(playerCamera.transform);
            Canvas canvas = flashObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = playerCamera;
            
            Image flashImage = flashObj.AddComponent<Image>();
            flashImage.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0.3f);
            flashImage.rectTransform.anchorMin = Vector2.zero;
            flashImage.rectTransform.anchorMax = Vector2.one;
            flashImage.rectTransform.sizeDelta = Vector2.zero;
            
            yield return new WaitForSeconds(damageFlashDuration);
            
            Destroy(flashObj);
        }
    }
    
    // ตาย
    void Die()
    {
        if (isDead) return;
        isDead = true;

    // หยุดเสียงทุกอย่างในเกม
        AudioListener.pause = true;

    // เล่นเสียง Game Over แยก
        if (gameOverSound != null)
        {
            AudioSource tempAudio = gameObject.AddComponent<AudioSource>();
            tempAudio.ignoreListenerPause = true; // เล่นได้แม้ Listener pause
            tempAudio.PlayOneShot(gameOverSound);
        }

        if (deathPanel)
        {
            deathPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (controller) controller.enabled = false;

        FPSGunSystem gunSystem = GetComponentInChildren<FPSGunSystem>();
        if (gunSystem) gunSystem.enabled = false;

        foreach (MonoBehaviour script in GetComponents<MonoBehaviour>())
        {
            if (script != this)
                script.enabled = false;
        }

        StartCoroutine(ForceUnlockCursor());
    }
    IEnumerator ForceUnlockCursor()
    {
        yield return null;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log($"Cursor State: {Cursor.lockState}, Visible: {Cursor.visible}");
    }
    

}