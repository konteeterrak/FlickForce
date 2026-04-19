using UnityEngine;

public class ResetSystem : MonoBehaviour
{
    // ค่าเริ่มต้นเป็น 0
    public int structure = 0;
    public int energy = 0;
    public int thrust = 0;

    void Start()
    {
        // รีเซ็ตค่าเมื่อกด Run
        structure = 100;
        energy = 75;
        thrust = 50;

        // แสดงผลทาง Console
  
        Debug.Log("Structure (ความทนทาน): " + structure);
        Debug.Log("Energy (พลังงาน): " + energy);
        Debug.Log("Thrust (แรงขับ): " + thrust);

    }
}
