using UnityEngine;

public class TestBullet : MonoBehaviour
{
    public GameObject bulletPrefab;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (bulletPrefab != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                Debug.Log("สร้างกระสุนทดสอบ: " + bullet.name);
            }
            else
            {
                Debug.LogError("Bullet Prefab is NULL!");
            }
        }
    }
}