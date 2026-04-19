using UnityEngine;

public class Hit_active : MonoBehaviour
{
    public GameObject objecttoActivate;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            objecttoActivate.SetActive(true);
        }
    }
}
