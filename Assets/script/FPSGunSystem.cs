using UnityEngine;
using System.Collections;

public class FPSGunSystem : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bulletPrefab; // Cube bullet prefab
    public Transform gunBarrel; // จุดที่กระสุนออกจากปืน
    public GameObject gunModel; // โมเดลปืน
    
    [Header("Shooting Settings")]
    public float bulletSpeed = 50f;
    public float bulletLifetime = 5f;
    public int maxAmmo = 30;
    public int currentAmmo = 30;
    public float reloadTime = 2f;
    public float fireRate = 0.1f;
    
    [Header("Recoil Settings")]
    public float recoilAmount = 0.5f;
    public float recoilSpeed = 10f;
    public float returnSpeed = 5f;
    
    [Header("Mouse Sensitivity")]
    public float mouseSensitivity = 2f;
    public float maxRecoilMultiplier = 2f; // ถ้าสะบัดแรงจะมี recoil เยอะ
    
    [Header("Crosshair Settings")]
    public Camera playerCamera; // กล้องของผู้เล่น
    public float shootRange = 100f; // ระยะยิงสูงสุด
    
    [Header("Crosshair UI")]
    public Texture2D crosshairTexture; // รูป crosshair (ถ้ามี)
    public float crosshairSize = 20f;
    public Color crosshairColor = Color.white;
    
    [Header("Audio Settings")]
    public AudioClip shootSound; // เสียงยิง
    public AudioClip reloadSound; // เสียงรีโหลด
    public AudioClip emptySound; // เสียงกระสุนหมด
    private AudioSource audioSource;
    
    [Header("Aim Down Sight (ADS) Settings")]
    public bool canAim = true; // เปิด/ปิดการเล็ง
    public Vector3 aimPosition = new Vector3(0f, -0.1f, 0.3f); // ตำแหน่งปืนตอนเล็ง
    public float aimFOV = 40f; // FOV กล้องตอนเล็ง (ซูมเข้า)
    public float aimSpeed = 8f; // ความเร็วในการเล็ง
    public float aimSpreadMultiplier = 0.3f; // ลด spread ตอนเล็ง (ยิงแม่นขึ้น)
    
    private Vector3 originalGunPosition;
    private float originalFOV;
    private bool isAiming = false;
    
    private bool isReloading = false;
    private float nextFireTime = 0f;
    private Vector3 currentRecoil;
    private Vector3 targetRecoil;
    private float lastMouseX;
    private float mouseSpeed;
    
    void Start()
    {
        // ตรวจสอบว่าอยู่ใน Scene เกมหรือไม่
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Menuscene")
        {
            Debug.LogWarning("Gun System ถูกปิดการใช้งานใน Menuscene");
            this.enabled = false;
            return;
        }
        
        if (gunBarrel == null)
        {
            Debug.LogError("กรุณาใส่ Gun Barrel Transform!");
        }
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        // เพิ่ม AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
        
        // บันทึกค่าเริ่มต้น
        originalGunPosition = transform.localPosition;
        originalFOV = playerCamera.fieldOfView;
        
        currentAmmo = maxAmmo;
        
        // ซ่อน cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        // ให้ปืนหมุนตามกล้อง (ถ้าไม่ได้เป็นลูกของกล้อง)
        if (transform.parent != playerCamera.transform)
        {
            transform.rotation = playerCamera.transform.rotation;
            Vector3 targetPos = playerCamera.transform.position 
                                + playerCamera.transform.forward * 0.5f 
                                + playerCamera.transform.right * 0.3f 
                                + playerCamera.transform.up * -0.2f;

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 30f);
        }
        // ตรวจสอบการกดปุ่มเล็ง
        if (canAim && !isReloading)
        {
            if (Input.GetMouseButton(1)) // กดค้าง RMB
            {
                isAiming = true;
            }
            else
            {
                isAiming = false;
            }
        }
        
        // คำนวณความเร็วการเคลื่อนที่ของเมาส์
        float currentMouseX = Input.GetAxis("Mouse X");
        mouseSpeed = Mathf.Abs(currentMouseX - lastMouseX) * mouseSensitivity;
        lastMouseX = currentMouseX;
        
        // ยิง (เฉพาะ M1 เท่านั้น)
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime && !isReloading)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
            else
            {
                // เล่นเสียงกระสุนหมด
                PlaySound(emptySound);
                Debug.Log("กระสุนหมด! กด R เพื่อรีโหลด");
            }
        }
        
        // รีโหลด (กด R เท่านั้น - ไม่มี auto reload)
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
        {
            StartCoroutine(Reload());
        }
        
        // จัดการ Recoil
        HandleRecoil();
    }
    
    void LateUpdate()
    {
        if (canAim)
        {
        // แก้ตรงนี้ - ใช้ smooth lerp ที่นุ่มนวลขึ้น
            float aimLerp = 1f - Mathf.Exp(-aimSpeed * Time.deltaTime);
        
            if (isAiming)
            {
                playerCamera.fieldOfView = Mathf.Lerp(
                    playerCamera.fieldOfView, 
                    aimFOV, 
                    aimLerp
                );
            }
            else
            {
                playerCamera.fieldOfView = Mathf.Lerp(
                    playerCamera.fieldOfView, 
                    originalFOV, 
                    aimLerp
                );
            }
        }
    }
    
    void Shoot()
    {
        currentAmmo--; // แก้จาก currentHealth
        
        // ตรวจสอบว่า gunBarrel มีค่าหรือไม่
        if (gunBarrel == null)
        {
            Debug.LogError("Gun Barrel is null! ไม่สามารถยิงได้");
            return;
        }
        if (bulletPrefab == null)
        {
            Debug.LogError("bulletPrefab เป็น null! กรุณา Assign Prefab ใน Inspector");
            return;
        }
        
        // ยิงแบบ Raycast จากกล้องไปที่จุดกึ่งกลางหน้าจอ (crosshair)
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        Vector3 targetPoint;
        
        // ถ้า Raycast ชนอะไร
        if (Physics.Raycast(ray, out hit, shootRange))
        {
            targetPoint = hit.point;
            
            // ❌ ลบส่วนนี้ออก - ไม่ให้ Raycast ทำดาเมจ
            // ให้กระสุนทำดาเมจแทน
            /*
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(25f);
                }
            }
            */
        }
        else
        {
            // ถ้าไม่ชนอะไร ยิงไปข้างหน้า
            targetPoint = ray.GetPoint(shootRange);
        }
        
        
        
        GameObject bullet = Instantiate(bulletPrefab, gunBarrel.position, Quaternion.identity);
        
        // ตรวจสอบว่าสร้างสำเร็จ
        if (bullet == null)
        {
            Debug.LogError("ไม่สามารถสร้างกระสุนได้!");
            return;
        }
        
        Debug.Log($"สร้างกระสุนที่ตำแหน่ง: {gunBarrel.position}");
        
        // คำนวณทิศทางจาก gunBarrel ไปยัง targetPoint
        Vector3 shootDirection = (targetPoint - gunBarrel.position).normalized;
        
        // เพิ่ม spread จากการสะบัดเมาส์ (ลดลงเมื่อเล็ง)
        float spread = mouseSpeed * maxRecoilMultiplier * 0.05f;
        if (isAiming)
        {
            spread *= aimSpreadMultiplier; // ลด spread ตอนเล็ง = ยิงแม่นขึ้น
        }
        
        shootDirection += new Vector3(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            Random.Range(-spread, spread)
        );
        shootDirection.Normalize();
        
        // หมุนกระสุนให้หันไปทาง targetPoint
        bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
        
        // เพิ่ม Rigidbody
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = bullet.AddComponent<Rigidbody>();
            Debug.Log("เพิ่ม Rigidbody ให้กระสุน");
        }
        
        rb.useGravity = true;
        rb.velocity = shootDirection * bulletSpeed;
        
        // เพิ่ม Bullet Script
        BulletBehavior bulletScript = bullet.GetComponent<BulletBehavior>();
        if (bulletScript == null)
        {
            bulletScript = bullet.AddComponent<BulletBehavior>();
            Debug.Log("เพิ่ม BulletBehavior ให้กระสุน");
        }
        bulletScript.lifetime = bulletLifetime;
        
        // Recoil
        float recoilMultiplier = 1f + (mouseSpeed * maxRecoilMultiplier);
        targetRecoil += new Vector3(-recoilAmount * recoilMultiplier, 0, 0);
        
        // เล่นเสียงยิง
        PlaySound(shootSound);
        
        Debug.Log($"ยิง! เหลือกระสุน: {currentAmmo} | Spread: {spread:F2}");
    }
    
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("กำลังรีโหลด...");
        
        // เล่นเสียงรีโหลด
        PlaySound(reloadSound);
        
        // Animation reload ปืน (สามารถเพิ่ม animation ได้)
        if (gunModel != null)
        {
            Vector3 originalPos = gunModel.transform.localPosition;
            gunModel.transform.localPosition += Vector3.down * 0.2f;
            
            yield return new WaitForSeconds(reloadTime);
            
            gunModel.transform.localPosition = originalPos;
        }
        else
        {
            yield return new WaitForSeconds(reloadTime);
        }
        
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("รีโหลดเสร็จ!");
    }
    
    // ฟังก์ชันเล่นเสียง
    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    void HandleRecoil()
    {
        // ค่อยๆ ไปที่ target recoil
        currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, recoilSpeed * Time.deltaTime);
        
        // ค่อยๆ กลับมาที่ตำแหน่งเดิม
        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, returnSpeed * Time.deltaTime);
        
        // ใช้ recoil กับกล้อง
        transform.localRotation = Quaternion.Euler(currentRecoil);
    }
    
    // แสดง UI ง่ayๆ
    void OnGUI()
    {
        // แสดงข้อมูลกระสุน
        if (!enabled || !gameObject.activeInHierarchy) return;
        GUI.Label(new Rect(10, 10, 200, 30), $"กระสุน: {currentAmmo} / {maxAmmo}");
        if (isReloading)
        {
            GUI.Label(new Rect(10, 40, 200, 30), "กำลังรีโหลด...");
        }
        if (isAiming)
        {
            GUI.Label(new Rect(10, 70, 200, 30), "[ กำลังเล็ง ]");
        }
        
        // วาด Crosshair ตรงกลางจอ
        float centerX = Screen.width / 2f;
        float centerY = Screen.height / 2f;
        
        // ปรับขนาด crosshair ตอนเล็ง
        float dynamicCrosshairSize = isAiming ? crosshairSize * 0.5f : crosshairSize;
        
        if (crosshairTexture != null)
        {
            // ถ้ามีรูป crosshair ใช้รูป
            GUI.DrawTexture(
                new Rect(centerX - dynamicCrosshairSize / 2, centerY - dynamicCrosshairSize / 2, dynamicCrosshairSize, dynamicCrosshairSize),
                crosshairTexture
            );
        }
        else
        {
            // วาด crosshair แบบง่ายๆ (เส้นกาง)
            float crosshairLength = dynamicCrosshairSize;
            float crosshairThickness = 2f;
            
            // เปลี่ยนสี crosshair
            Color dynamicColor = crosshairColor;
            if (isAiming)
            {
                dynamicColor = Color.green; // สีเขียวตอนเล็ง
            }
            else if (mouseSpeed > 1f)
            {
                dynamicColor = Color.Lerp(crosshairColor, Color.red, mouseSpeed * 0.2f);
            }
            
            GUI.color = dynamicColor;
            
            // เส้นแนวนอน
            GUI.DrawTexture(
                new Rect(centerX - crosshairLength / 2, centerY - crosshairThickness / 2, crosshairLength, crosshairThickness),
                Texture2D.whiteTexture
            );
            
            // เส้นแนวตั้ง
            GUI.DrawTexture(
                new Rect(centerX - crosshairThickness / 2, centerY - crosshairLength / 2, crosshairThickness, crosshairLength),
                Texture2D.whiteTexture
            );
            
            // จุดตรงกลาง
            GUI.DrawTexture(
                new Rect(centerX - 1, centerY - 1, 2, 2),
                Texture2D.whiteTexture
            );
            
            GUI.color = Color.white;
        }
    }
    // เพิ่มฟังก์ชันนี้เข้าไปใน FPSGunSystem.cs
    void OnDisable()
    {
    // เมื่อ Script ถูกปิด ให้ปลด Lock Cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isAiming = false;
    }
}