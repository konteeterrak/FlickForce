using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Visual Feedback")]
    public Color normalColor = Color.white;
    public Color hitColor = Color.red;
    public float hitFlashDuration = 0.1f;
    
    private Renderer enemyRenderer;
    private bool isFlashing = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        enemyRenderer = GetComponent<Renderer>();
        
        if (enemyRenderer == null)
        {
            Debug.LogWarning($"Enemy {gameObject.name} ไม่มี Renderer!");
        }
    }
    
    // ฟังก์ชันรับดาเมจ
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        Debug.Log($"{gameObject.name} โดนยิง! HP: {currentHealth}/{maxHealth}");
        
        // แสดง visual feedback
        if (!isFlashing && enemyRenderer != null)
        {
            StartCoroutine(HitFlash());
        }
        
        // ตรวจสอบว่าตายหรือยัง
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // ฟังก์ชันตาย
    void Die()
    {
        Debug.Log($"{gameObject.name} ตายแล้ว!");
        
        // สามารถเพิ่ม effect ตอนตายได้ที่นี่
        // เช่น particle effect, sound effect, drop item, เพิ่มคะแนน
        
        Destroy(gameObject);
    }
    
    // แสดงเอฟเฟกต์กระพริบเมื่อโดนยิง
    System.Collections.IEnumerator HitFlash()
    {
        isFlashing = true;
        
        if (enemyRenderer != null)
        {
            Color originalColor = enemyRenderer.material.color;
            enemyRenderer.material.color = hitColor;
            
            yield return new WaitForSeconds(hitFlashDuration);
            
            enemyRenderer.material.color = originalColor;
        }
        
        isFlashing = false;
    }
    
    // แสดง Health Bar ง่ายๆ เหนือศัตรู
    void OnGUI()
    {
        if (Camera.main == null) return;
        
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f);
        
        if (screenPos.z > 0) // ต้องอยู่หน้ากล้อง
        {
            float healthBarWidth = 50f;
            float healthBarHeight = 5f;
            float healthPercent = currentHealth / maxHealth;
            
            // Background
            GUI.color = Color.black;
            GUI.DrawTexture(
                new Rect(screenPos.x - healthBarWidth / 2, Screen.height - screenPos.y, healthBarWidth, healthBarHeight),
                Texture2D.whiteTexture
            );
            
            // Health bar
            if (healthPercent > 0.5f)
                GUI.color = Color.green;
            else if (healthPercent > 0.25f)
                GUI.color = Color.yellow;
            else
                GUI.color = Color.red;
                
            GUI.DrawTexture(
                new Rect(screenPos.x - healthBarWidth / 2, Screen.height - screenPos.y, healthBarWidth * healthPercent, healthBarHeight),
                Texture2D.whiteTexture
            );
            
            GUI.color = Color.white;
        }
    }
}