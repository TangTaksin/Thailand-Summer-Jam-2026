using UnityEngine;

public class ScreenElements : MonoBehaviour
{
    protected Rigidbody2D rb2D;
    Collider2D _collider;
    protected Vector3 cursorOffset;
    
    public enum ScreenElementState
    {
        Normal,
        Freeze
    }
    protected ScreenElementState element_state;

    protected virtual void Start()
    {
        TryGetComponent<Rigidbody2D>(out rb2D);
        TryGetComponent<Collider2D>(out _collider);
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

    #region Getter
    
    public Collider2D GetCollider()
    {
        return _collider;
    }

    #endregion
}
