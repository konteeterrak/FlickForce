using UnityEngine;

public class HorrorCameraEffects : MonoBehaviour
{
    [Header("Camera Reference")]
    public Camera mainCamera;
    public Transform cameraTransform;
    
    [Header("Head Bob (การโยกหัว)")]
    public bool enableHeadBob = true;
    public float bobFrequency = 2f; // ความถี่การโยก
    public float bobHorizontalAmount = 0.03f; // แกว่งซ้าย-ขวา
    public float bobVerticalAmount = 0.05f; // แกว่งบน-ล่าง
    private float bobTimer = 0f;
    private Vector3 originalCameraPosition;
    
    [Header("Camera Sway (การโยนกล้อง)")]
    public bool enableCameraSway = true;
    public float swayAmount = 0.02f;
    public float swaySpeed = 5f;
    public float maxSwayAngle = 3f;
    
    [Header("Breathing Effect (หายใจ)")]
    public bool enableBreathing = true;
    public float breathingSpeed = 0.8f;
    public float breathingAmount = 0.01f;
    private float breathingTimer = 0f;
    
    [Header("Fear Effect (ความกลัว)")]
    [Range(0f, 1f)]
    public float fearLevel = 0f; // 0 = ปกติ, 1 = กลัวมาก
    public float fearShakeAmount = 0.1f;
    public float fearFOVIncrease = 10f;
    public float fearVignetteIntensity = 0.5f;
    
    [Header("Footstep Shake (สั่นตอนเดิน)")]
    public bool enableFootstepShake = true;
    public float footstepShakeAmount = 0.02f;
    
    [Header("Look Around Inertia (กล้องมีน้ำหนัก)")]
    public bool enableInertia = true;
    public float inertiaAmount = 0.05f;
    public float inertiaSpeed = 3f;
    private Vector3 inertiaCameraRotation;
    
    [Header("Ambient Settings")]
    public float darkFOV = 70f; // FOV ตอนมืด
    public float defaultFOV = 60f;
    
    private CharacterController characterController;
    private Vector3 lastMousePosition;
    private float defaultCameraFOV;
    
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        
        if (cameraTransform == null)
            cameraTransform = mainCamera.transform;
        
        originalCameraPosition = cameraTransform.localPosition;
        defaultCameraFOV = mainCamera.fieldOfView;
        
        characterController = GetComponentInParent<CharacterController>();
        lastMousePosition = Input.mousePosition;
    }
    
    void Update()
    {
        Vector3 cameraOffset = Vector3.zero;
        
        // Head Bob (โยกหัวตอนเดิน)
        if (enableHeadBob)
        {
            cameraOffset += CalculateHeadBob();
        }
        
        // Breathing Effect
        if (enableBreathing)
        {
            cameraOffset += CalculateBreathing();
        }
        
        // Camera Sway (โยนกล้องตาม Mouse)
        if (enableCameraSway)
        {
            ApplyCameraSway();
        }
        
        // Inertia Effect
        if (enableInertia)
        {
            ApplyCameraInertia();
        }
        
        // Fear Effect
        ApplyFearEffect();
        
        // ใช้ Offset กับกล้อง
        cameraTransform.localPosition = originalCameraPosition + cameraOffset;
    }
    
    // ===== HEAD BOB =====
    Vector3 CalculateHeadBob()
    {
        if (characterController == null) return Vector3.zero;
        
        // เช็คว่ากำลังเคลื่อนที่หรือไม่
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isMoving = (horizontal != 0 || vertical != 0) && characterController.isGrounded;
        
        if (isMoving)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            
            // คำนวณการโยก
            float bobX = Mathf.Sin(bobTimer) * bobHorizontalAmount;
            float bobY = Mathf.Sin(bobTimer * 2f) * bobVerticalAmount; // เร็วกว่า 2 เท่า
            
            // Footstep Shake (สั่นเมื่อเท้าแตะพื้น)
            if (enableFootstepShake && Mathf.Sin(bobTimer * 2f) < -0.9f)
            {
                bobY -= footstepShakeAmount * Random.Range(0.5f, 1f);
            }
            
            return new Vector3(bobX, bobY, 0);
        }
        else
        {
            bobTimer = 0;
            return Vector3.zero;
        }
    }
    
    // ===== BREATHING =====
    Vector3 CalculateBreathing()
    {
        breathingTimer += Time.deltaTime * breathingSpeed;
        float breathingOffset = Mathf.Sin(breathingTimer) * breathingAmount;
        
        return new Vector3(0, breathingOffset, 0);
    }
    
    // ===== CAMERA SWAY =====
    void ApplyCameraSway()
    {
        // เคลื่อนไหวเมาส์
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        // Sway ตาม Mouse
        Quaternion targetRotation = Quaternion.Euler(
            mouseY * swayAmount * -1f,
            mouseX * swayAmount,
            mouseX * maxSwayAngle * -1f // Tilt ตาม Mouse
        );
        
        cameraTransform.localRotation = Quaternion.Slerp(
            cameraTransform.localRotation,
            targetRotation,
            Time.deltaTime * swaySpeed
        );
    }
    
    // ===== INERTIA (กล้องมีน้ำหนัก) =====
    void ApplyCameraInertia()
    {
        Vector3 mouseDelta = (Vector3)Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;
        
        // เพิ่ม Inertia Rotation
        inertiaCameraRotation.x -= mouseDelta.y * inertiaAmount;
        inertiaCameraRotation.y += mouseDelta.x * inertiaAmount;
        
        // ค่อยๆ กลับมาที่ศูนย์
        inertiaCameraRotation = Vector3.Lerp(inertiaCameraRotation, Vector3.zero, Time.deltaTime * inertiaSpeed);
        
        // ใช้กับกล้อง
        cameraTransform.localRotation *= Quaternion.Euler(inertiaCameraRotation.x, inertiaCameraRotation.y, 0);
    }
    
    // ===== FEAR EFFECT =====
    void ApplyFearEffect()
    {
        if (fearLevel > 0f)
        {
            // สั่นกล้อง
            Vector3 fearShake = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0
            ) * fearShakeAmount * fearLevel;
            
            cameraTransform.localPosition += fearShake;
            
            // เพิ่ม FOV (Panic)
            float targetFOV = defaultCameraFOV + (fearFOVIncrease * fearLevel);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * 2f);
        }
        else
        {
            // กลับมา FOV ปกติ
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultCameraFOV, Time.deltaTime * 2f);
        }
    }
    
    // ===== PUBLIC FUNCTIONS =====
    
    // เพิ่มความกลัว (เรียกจาก Script อื่น)
    public void SetFearLevel(float level)
    {
        fearLevel = Mathf.Clamp01(level);
    }
    
    // สั่นกล้องแบบกระทันหัน (เช่น เห็นผี)
    public void ShakeCamera(float intensity, float duration)
    {
        StartCoroutine(CameraShakeCoroutine(intensity, duration));
    }
    
    System.Collections.IEnumerator CameraShakeCoroutine(float intensity, float duration)
    {
        float timer = 0f;
        
        while (timer < duration)
        {
            Vector3 randomShake = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ) * intensity;
            
            cameraTransform.localPosition = originalCameraPosition + randomShake;
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        // กลับมาตำแหน่งเดิม
        cameraTransform.localPosition = originalCameraPosition;
    }
    
    // Slow Motion Effect (เวลาเห็นผี)
    public void SlowMotion(float timeScale, float duration)
    {
        StartCoroutine(SlowMotionCoroutine(timeScale, duration));
    }
    
    System.Collections.IEnumerator SlowMotionCoroutine(float timeScale, float duration)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}