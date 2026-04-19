using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ScreenMateMovement : ScreenElements
{
    // สร้างหมวดหมู่สถานะ (State) ของตัวละคร
    public enum MateState
    {
        Walk,
        Idle,
        Falling
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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; 
        SetWalkState();
    }

    private void FixedUpdate()
    {
        if (element_state != ScreenElementState.Normal) return;

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
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}