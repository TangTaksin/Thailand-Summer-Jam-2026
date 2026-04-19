using UnityEngine;

public class ScreenElements : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private Vector3 cursorOffset;
    
    public enum ScreenElementState
    {
        Normal,
        Freeze
    }
    protected ScreenElementState element_state;

    void Start()
    {
        TryGetComponent<Rigidbody2D>(out rb2D);
    }

    public void StateOverride(ScreenElementState state)
    {
        element_state = state;
    }

    public void UpdateOffset(Vector3 mouseClickPos)
    {
        cursorOffset = transform.position - mouseClickPos;
    }

    public void Drag(Vector3 mousePos, Vector3 mouseVelo)
    {
        transform.position = new Vector3(mousePos.x + cursorOffset.x, mousePos.y + cursorOffset.y, transform.position.z);
    }

}
