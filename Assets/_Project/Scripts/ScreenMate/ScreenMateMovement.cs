using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ScreenMateMovement : MonoBehaviour
{
    // สร้างหมวดหมู่สถานะ (State) ของตัวละคร
    public enum MateState
    {
        Walk,
        Idle,
        Falling,
        Dragged
    }

    [Header("Current State")]
    public MateState currentState = MateState.Walk;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float leftBound = -7f;
    public float rightBound = 7f;

    [Header("State Settings")]
    public float minWalkTime = 2f;
    public float maxWalkTime = 6f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;

    private int movingDirection = 1;
    private Rigidbody2D rb;
    private Vector3 offset;
    private float stateTimer = 0f;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    //keep
    private bool _isGameOver = false;

    //keep
    private void OnEnable()
    {
        ActionCommands.OnGameOver += HandleGameOver;
    }

    //keep
    private void OnDisable()
    {
        ActionCommands.OnGameOver -= HandleGameOver;
    }


    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        SetWalkState();
    }

    private void FixedUpdate()
    {
        if (_isGameOver) return;
        if (currentState == MateState.Dragged) return;

        if (rb.linearVelocity.y < -0.1f)
        {
            currentState = MateState.Falling;
        }
        else if (currentState == MateState.Falling && Mathf.Abs(rb.linearVelocity.y) <= 0.05f)
        {
            SetWalkState();
        }

        switch (currentState)
        {
            case MateState.Walk:
                HandleStateTimer();
                Patrol();
                break;

            case MateState.Idle:
                HandleStateTimer();
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;

            case MateState.Falling:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;
        }
    }

    private void HandleStateTimer()
    {
        stateTimer -= Time.fixedDeltaTime;
        if (stateTimer <= 0)
        {
            if (currentState == MateState.Idle)
            {
                SetWalkState();
            }
            else if (currentState == MateState.Walk)
            {
                SetIdleState();
            }
        }
    }

    private void SetWalkState()
    {
        currentState = MateState.Walk;
        stateTimer = Random.Range(minWalkTime, maxWalkTime);
    }

    private void SetIdleState()
    {
        currentState = MateState.Idle;
        stateTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    private void Patrol()
    {
        rb.linearVelocity = new Vector2(movingDirection * moveSpeed, rb.linearVelocity.y);

        if (transform.position.x >= rightBound && movingDirection == 1)
        {
            movingDirection = -1;
            FlipSprite();
        }
        else if (transform.position.x <= leftBound && movingDirection == -1)
        {
            movingDirection = 1;
            FlipSprite();
        }
    }

    private void FlipSprite()
    {
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
    }

    //keep
    private void HandleGameOver()
    {
        _isGameOver = true;
        rb.linearVelocity = Vector2.zero;
        currentState = MateState.Idle;
        Debug.Log("[ScreenMateMovement] Game Over — Movement stopped.");
    }

    private void OnMouseDown()
    {
        if (_isGameOver) return;
        currentState = MateState.Dragged;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

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
        rb.bodyType = RigidbodyType2D.Dynamic;
        currentState = MateState.Falling;
    }
}