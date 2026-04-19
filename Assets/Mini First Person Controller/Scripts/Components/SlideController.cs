using UnityEngine;

public class SlideController : MonoBehaviour
{
    [Header("Slide Key")]
    public KeyCode slideKey = KeyCode.C;
    
    [Header("Slide Settings")]
    public float slideSpeed = 10f; // ความเร็ว Slide
    public float slideDuration = 0.8f; // ระยะเวลา Slide
    public float slideCooldown = 1f; // Cooldown ก่อน Slide ใหม่
    public float minSpeedToSlide = 3f; // ความเร็วขั้นต่ำที่ต้องวิ่งก่อน Slide
    
    [Header("Crouch Settings")]
    public Transform headToLower; // กล้อง
    public float crouchYHeadPosition = 1f; // ความสูงหัวตอนย่อ
    public CapsuleCollider capsuleCollider;
    public float crouchColliderHeight = 1f; // ความสูง Collider ตอนย่อ
    
    [Header("Movement Component")]
    public FirstPersonMovement movement; // ถ้ามี Script นี้
    public CharacterController characterController;
    
    private bool isSliding = false;
    private bool canSlide = true;
    private bool isRunning = false; // เพิ่มตัวแปรนี้
    private float slideTimer = 0f;
    private float cooldownTimer = 0f;
    private float runTimer = 0f; // เวลาที่วิ่ง
    private Vector3 slideDirection;
    private float defaultHeadY;
    private float defaultColliderHeight;
    private Vector3 defaultColliderCenter;
    
    void Start()
    {
        // เก็บค่าเริ่มต้น
        if (headToLower)
            defaultHeadY = headToLower.localPosition.y;
        
        if (capsuleCollider)
        {
            defaultColliderHeight = capsuleCollider.height;
            defaultColliderCenter = capsuleCollider.center;
        }
        
        // ถ้าไม่ได้ใส่ CharacterController
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        // อัปเดต Cooldown
        if (!canSlide)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                canSlide = true;
            }
        }
        
        // เช็คว่ากำลังวิ่งหรือไม่
        CheckIfRunning();
        
        // เช็คการกด Slide
        if (Input.GetKeyDown(slideKey) && canSlide && !isSliding)
        {
            TryStartSlide();
        }
        
        // จัดการ Slide
        if (isSliding)
        {
            HandleSlide();
        }
    }
    
    void CheckIfRunning()
    {
        // เช็คว่ากำลังกดปุ่มเคลื่อนที่หรือไม่
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isMoving = (horizontal != 0 || vertical != 0);
        
        // เช็คว่ากำลังกด Shift (วิ่ง) หรือไม่ (ปรับตามเกมของคุณ)
        bool isPressingRun = Input.GetKey(KeyCode.LeftShift);
        
        if (isMoving && isPressingRun)
        {
            runTimer += Time.deltaTime;
            isRunning = runTimer >= 0.2f; // วิ่งไปแล้ว 0.2 วินาที
        }
        else
        {
            runTimer = 0f;
            isRunning = false;
        }
    }
    
    void TryStartSlide()
    {
        // วิธีที่ 1: เช็คจาก CharacterController (ถ้ามี)
        if (characterController != null)
        {
            Vector3 velocity = characterController.velocity;
            velocity.y = 0; // ไม่สนใจความเร็วแนวตั้ง
            float currentSpeed = velocity.magnitude;
            bool isGrounded = characterController.isGrounded;
            
            if (currentSpeed >= minSpeedToSlide && isGrounded)
            {
                StartSlide();
                return;
            }
        }
        
        // วิธีที่ 2: เช็คจากการวิ่ง (ถ้าไม่มี CharacterController)
        if (isRunning)
        {
            StartSlide();
            return;
        }
        
        // ถ้าไม่วิ่งเร็วพอ ให้ย่อแทน
        StartCrouch();
    }
    
    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        
        // เก็บทิศทางที่กำลังวิ่ง
        if (characterController != null)
        {
            slideDirection = new Vector3(characterController.velocity.x, 0, characterController.velocity.z).normalized;
        }
        else
        {
            slideDirection = transform.forward;
        }
        
        // ลดความสูง
        LowerBody();
        
        Debug.Log("Slide Started!");
    }
    
    void HandleSlide()
    {
        slideTimer -= Time.deltaTime;
        
        // เคลื่อนที่ไปข้างหน้า
        if (characterController != null)
        {
            // ลด Friction (ความฝืดเหมือนลื่นไป)
            float speedMultiplier = slideTimer / slideDuration; // ลดความเร็วค่อยๆ
            Vector3 movement = slideDirection * slideSpeed * speedMultiplier * Time.deltaTime;
            characterController.Move(movement);
        }
        
        // จบ Slide
        if (slideTimer <= 0)
        {
            EndSlide();
        }
        
        // ยกเลิก Slide ถ้ากดกระโดด (ถ้ามี Jump)
        if (Input.GetButtonDown("Jump"))
        {
            EndSlide();
        }
    }
    
    void EndSlide()
    {
        isSliding = false;
        canSlide = false;
        cooldownTimer = slideCooldown;
        
        // ยืนขึ้น
        RiseBody();
        
        Debug.Log("Slide Ended!");
    }
    
    void StartCrouch()
    {
        // ย่อแบบธรรมดา (ไม่มี Slide)
        LowerBody();
        Debug.Log("Crouching...");
    }
    
    void LowerBody()
    {
        // ลดความสูงหัว
        if (headToLower)
        {
            headToLower.localPosition = new Vector3(
                headToLower.localPosition.x,
                crouchYHeadPosition,
                headToLower.localPosition.z
            );
        }
        
        // ลดความสูง Collider
        if (capsuleCollider)
        {
            capsuleCollider.height = crouchColliderHeight;
            capsuleCollider.center = new Vector3(0, crouchColliderHeight * 0.5f, 0);
        }
    }
    
    void RiseBody()
    {
        // ยกหัวขึ้น
        if (headToLower)
        {
            headToLower.localPosition = new Vector3(
                headToLower.localPosition.x,
                defaultHeadY,
                headToLower.localPosition.z
            );
        }
        
        // ยก Collider ขึ้น
        if (capsuleCollider)
        {
            capsuleCollider.height = defaultColliderHeight;
            capsuleCollider.center = defaultColliderCenter;
        }
    }
    
    // สำหรับแสดงสถานะ
    void OnGUI()
    {
        if (isSliding)
        {
            GUI.Label(new Rect(10, 100, 200, 30), "SLIDING!");
        }
        
        if (!canSlide)
        {
            GUI.Label(new Rect(10, 130, 200, 30), $"Cooldown: {cooldownTimer:F1}s");
        }
    }
}