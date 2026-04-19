using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void Restart()
    {
        AudioListener.pause = false; // เปิดเสียงกลับ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        AudioListener.pause = false; // เปิดเสียงกลับก่อนไป Menu
        SceneManager.LoadScene("MenuScene");
    }
}