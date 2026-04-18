using UnityEngine;

public class GoodFile : MonoBehaviour, IEffectable
{
    public float heal = 15f;

    private bool isDragging = false;
    private Vector3 offset;
    public void ApplyEffect(ScreenMateStats stats)
    {
        stats.UpdateCortisol(-heal);
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        isDragging = true;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y, transform.position.z);
    }

    private void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, transform.position.z);
    }

    private void OnMouseUp()
    {
        isDragging = false;

    }
}
