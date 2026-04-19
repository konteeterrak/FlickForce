using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTest : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    void Start()
    {
        Debug.Log($"ButtonTest พร้อมทำงานบน {gameObject.name}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("เมาส์อยู่บนปุ่ม!");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("ปุ่มถูกคลิก!");
    }
}