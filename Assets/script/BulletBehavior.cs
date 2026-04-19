using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float lifetime = 5f; // อายุกระสุน (วินาที)
    public float damage = 25f; // ดาเมจที่ทำให้ศัตรู
    
    private float spawnTime;
    private bool hasHit = false; // ป้องกันชนซ้ำ
    
    void Start()
    {
        
    }
    
    void Update()
    {
        // ตรวจสอบว่ากระสุนตกไปไกลเกินไปหรือไม่
        if (transform.position.y < -50f)
        {
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // ป้องกันการชนซ้ำ
        if (hasHit) return;
        hasHit = true;

        Debug.Log($"Bullet hit: {collision.gameObject.name}");
        
        // ตรวจสอบว่าชนอะไร
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("กระสุนโดน Enemy!");
            
            // ทำดาเมจให้ศัตรู (ใช้ EnemyAI แทน Enemy)
            EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("กระสุนชนกำแพง - ถูกทำลาย");
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("กระสุนชนพื้น - ถูกทำลาย");
        }
        else
        {
            Debug.Log($"กระสุนชน {collision.gameObject.name}");
        }
        
        // ทำลายกระสุนทันที
        Destroy(gameObject);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // กรณีใช้ Trigger แทน Collision
        if (hasHit) return;
        hasHit = true;
        
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        
        Destroy(gameObject);
    }
}