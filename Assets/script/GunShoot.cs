using UnityEngine;

public class GunShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform muzzle;
    public float bulletForce = 40f;

    [Header("Ammo")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 1.5f;
    bool isReloading = false;

    [Header("Accuracy")]
    public float baseSpread = 3f;      // กระสุนกระจายปกติ
    public float maxAccuracyBonus = 2f; // โบนัสความแม่นจากการสะบัด

    Vector2 lastMousePos;

    void Start()
    {
        currentAmmo = maxAmmo;
        lastMousePos = Input.mousePosition;
    }

    void Update()
    {
        if (isReloading) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        currentAmmo--;

        // คำนวณแรงสะบัดเมาส์
        Vector2 mouseDelta = (Vector2)Input.mousePosition - lastMousePos;
        float flickStrength = mouseDelta.magnitude;

        // สะบัดแรง = spread น้อยลง = แม่นขึ้น
        float spread = baseSpread - Mathf.Clamp(flickStrength * 0.05f, 0, maxAccuracyBonus);

        Vector3 direction = muzzle.forward;
        direction += Random.insideUnitSphere * spread * 0.01f;

        GameObject bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(direction.normalized * bulletForce, ForceMode.Impulse);

        lastMousePos = Input.mousePosition;
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
