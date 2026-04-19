using UnityEngine;

public class StairHelper : MonoBehaviour
{
    [Header("Stair Settings")]
    [Tooltip("ความสูงสูงสุดของบันไดที่ขึ้นได้")]
    public float maxStepHeight = 0.5f;
    
    [Tooltip("ระยะเช็คบันไดข้างหน้า")]
    public float stepCheckDistance = 0.5f;
    
    [Tooltip("แรงดันขึ้นบันได")]
    public float stepUpForce = 5f;
    
    [Tooltip("ความเร็วขึ้นบันไดเรียบ")]
    public float stepSmoothSpeed = 10f;
    
    private CharacterController controller;
    private Vector3 lastPosition;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastPosition = transform.position;
    }
    
    void Update()
    {
        if (controller == null) return;
        
        // เช็คว่ากำลังเคลื่อนที่หรือไม่
        Vector3 moveDirection = transform.position - lastPosition;
        moveDirection.y = 0;
        
        if (moveDirection.magnitude > 0.01f && controller.isGrounded)
        {
            CheckForStairs();
        }
        
        lastPosition = transform.position;
    }
    
    void CheckForStairs()
    {
        // ยิง Raycast ลงล่างข้างหน้า
        Vector3 rayStart = transform.position + Vector3.up * maxStepHeight;
        Vector3 rayDirection = transform.forward;
        
        RaycastHit hitLower;
        
        // เช็คว่ามีพื้นข้างหน้าหรือไม่
        if (Physics.Raycast(rayStart, rayDirection, out hitLower, stepCheckDistance))
        {
            // เช็คว่าพื้นสูงขึ้นหรือไม่ (บันได)
            float heightDifference = hitLower.point.y - transform.position.y;
            
            if (heightDifference > controller.stepOffset && heightDifference <= maxStepHeight)
            {
                // ขึ้นบันได
                StepUp(heightDifference);
            }
        }
    }
    
    void StepUp(float height)
    {
        // ดันขึ้นบันได
        Vector3 newPosition = transform.position;
        newPosition.y += height * Time.deltaTime * stepSmoothSpeed;
        
        // เคลื่อนที่ไปข้างหน้าเล็กน้อย
        newPosition += transform.forward * Time.deltaTime * stepUpForce;
        
        transform.position = newPosition;
    }
    
    // Debug: วาดเส้น Raycast
    void OnDrawGizmosSelected()
    {
        if (controller == null) return;
        
        Vector3 rayStart = transform.position + Vector3.up * maxStepHeight;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(rayStart, rayStart + transform.forward * stepCheckDistance);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(rayStart, 0.1f);
    }
}