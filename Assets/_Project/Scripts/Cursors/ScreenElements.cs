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
    public virtual bool IsGroupSelectable => true;

    protected virtual void Start()
    {
        TryGetComponent<Rigidbody2D>(out rb2D);
        TryGetComponent<Collider2D>(out _collider);
    }

    public virtual void StateOverride(ScreenElementState state)
    {
        element_state = state;

        switch (element_state)
        {
            case ScreenElementState.Normal:
                OnNormalState();
                break;
            case ScreenElementState.Freeze:
                OnFreezeState();
                break;
        }
    }

    protected virtual void OnNormalState()
    {
         if (rb2D != null)
            rb2D.gravityScale = 1f;
    }

    protected virtual void OnFreezeState()
    {
         if (rb2D != null)
         {
            rb2D.linearVelocity = Vector2.zero;
            rb2D.gravityScale = 0f;
        }
    }

    public void UpdateOffset(Vector3 mouseClickPos)
    {
        cursorOffset = transform.position - mouseClickPos;
    }

    public void Drag(Vector3 mousePos, Vector3 mouseVelo)
    {
        //เปลี่ยนเป็น MovePosition เพื่อไม่ขัด physics
        if (rb2D != null)
        {
            Vector2 targetPos = new Vector2(mousePos.x + cursorOffset.x, mousePos.y + cursorOffset.y);
            rb2D.MovePosition(targetPos);
        }
        else
        {
            transform.position = new Vector3(mousePos.x + cursorOffset.x, mousePos.y + cursorOffset.y, transform.position.z);
        }

    }

    #region Getter

    public Collider2D GetCollider()
    {
        return _collider;
    }

    #endregion
}
