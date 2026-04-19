using UnityEngine;

public class IgnitionInput : MonoBehaviour
{
    // ตัวแปรนับจำนวนครั้งที่กดปุ่ม E
    private int ignitionCount = 0;

    void Update()
    {
        // เช็คว่ามีการกดปุ่ม E หรือไม่
        if (Input.GetKeyDown(KeyCode.E))
        {
            ignitionCount++;

            // แสดงผลทาง Console
            Debug.Log("Ignition Attempt: " + ignitionCount);
        }
    }
}
