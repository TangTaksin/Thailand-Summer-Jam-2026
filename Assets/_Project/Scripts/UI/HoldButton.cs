using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Hold Settings")]
    public float holdDuration = 5f; // เวลาที่ต้องกดค้าง (วินาที)
    public Image progressImage;     // รูป UI ที่จะให้เลื่อนเป็น Progress

    [Header("Events")]
    public UnityEvent onHoldComplete; // Event เมื่อกดค้างสำเร็จ

    private bool _isHolding = false;
    private float _holdTimer = 0f;
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        ResetProgress();
    }

    private void Update()
    {
        // ทำงานเฉพาะตอนกำลังกดค้าง และปุ่มนั้นสามารถกดได้ (interactable = true)
        if (_isHolding && _button.interactable)
        {
            _holdTimer += Time.deltaTime;

            if (progressImage != null)
            {
                progressImage.fillAmount = _holdTimer / holdDuration;
            }

            if (_holdTimer >= holdDuration)
            {
                _isHolding = false;
                onHoldComplete?.Invoke(); // เรียกใช้งานฟังก์ชันที่ผูกไว้
                ResetProgress();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_button.interactable) return;
        _isHolding = true;
        _holdTimer = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetProgress();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetProgress(); // ยกเลิกถ้าย้ายเมาส์ออกนอกปุ่ม
    }

    private void OnDisable()
    {
        ResetProgress(); // รีเซ็ตทุกครั้งที่เมนูปิด (กันบั๊กกดค้างค้างไว้)
    }

    private void ResetProgress()
    {
        _isHolding = false;
        _holdTimer = 0f;
        if (progressImage != null) progressImage.fillAmount = 0f;
    }
}