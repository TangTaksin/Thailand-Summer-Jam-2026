using UnityEngine;

public class ManualFolderObject : MonoBehaviour
{
    [SerializeField] private TipManualUI _manualUISystem;

    // ฟังก์ชันนี้จะทำงานเมื่อคลิกที่ GameObject ที่มี Collider2D
    private void OnMouseDown()
    {
        if (_manualUISystem != null)
        {
            _manualUISystem.OpenManual();
        }
    }
}