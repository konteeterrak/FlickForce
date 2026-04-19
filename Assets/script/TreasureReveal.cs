using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TreasureReveal : MonoBehaviour
{
    [Header("Camera Zoom")]
    public Transform treasureObject;
    public float zoomDistance = 1.5f;
    public float zoomSpeed = 2f;
    public Vector3 startOffset = new Vector3(0, 2f, -6f);

    [Header("UI")]
    public GameObject infoPanel;
    public TextMeshProUGUI treasureName;
    public TextMeshProUGUI treasureDescription;
    public Button backButton;

    [Header("Treasure Info")]
    public string tName = "ดาบโบราณ";
    public string tDescription = "ดาบที่ซ่อนอยู่บนยอดหอคอยมาหลายร้อยปี...";

    [Header("Audio")]
    public AudioClip winnerSound;
    private AudioSource audioSource;

    private Vector3 targetPosition;
    private bool zoomDone = false;

    void Start()
    {
        transform.position = treasureObject.position + startOffset;
        transform.LookAt(treasureObject);

        infoPanel.SetActive(false);

        Vector3 dir = (transform.position - treasureObject.position).normalized;
        targetPosition = treasureObject.position + dir * zoomDistance;

        // ตั้งค่า AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        backButton.onClick.AddListener(() => SceneManager.LoadScene("Menuscene"));
    }

    void Update()
    {
        if (zoomDone) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            zoomSpeed * Time.deltaTime
        );
        transform.LookAt(treasureObject);

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            zoomDone = true;
            ShowTreasureInfo();
        }
    }

    void ShowTreasureInfo()
    {
        infoPanel.SetActive(true);
        treasureName.text = tName;
        treasureDescription.text = tDescription;

        // เล่นเสียง Winner
        if (winnerSound != null)
            audioSource.PlayOneShot(winnerSound);
    }
}