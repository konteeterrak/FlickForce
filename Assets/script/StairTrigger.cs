// StairTrigger.cs — แปะบน Empty Object ที่ปลายบันได
using UnityEngine;
using UnityEngine.SceneManagement;

public class StairTrigger : MonoBehaviour
{
    public string treasureSceneName = "TreasureScene";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(treasureSceneName);
        }
    }
}