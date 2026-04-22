using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ScreenMateMovement : ScreenElements
{
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
    private float stateTimer = 0f;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private bool _isGameOver = false;

    public override bool IsGroupSelectable => false;


    private void OnEnable()
    {
        ActionCommands.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        ActionCommands.OnGameOver -= HandleGameOver;
    }

    protected override void Start()
    {
        base.Start();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        rb2D.freezeRotation = true;
        SetWalkState();
    }

    private void FixedUpdate()
    {
        if (_isGameOver) return;
        if (element_state != ScreenElementState.Normal) return;

        if (rb2D.linearVelocity.y < -0.1f)
        {
            currentState = MateState.Falling;
        }
        else if (currentState == MateState.Falling && Mathf.Abs(rb2D.linearVelocity.y) <= 0.05f)
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
                rb2D.linearVelocity = new Vector2(0, rb2D.linearVelocity.y);
                break;

            case MateState.Falling:
                rb2D.linearVelocity = new Vector2(0, rb2D.linearVelocity.y);
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
        rb2D.linearVelocity = new Vector2(movingDirection * moveSpeed, rb2D.linearVelocity.y);

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
        rb2D.linearVelocity = Vector2.zero;
        currentState = MateState.Idle;
        Debug.Log("[ScreenMateMovement] Game Over — Movement stopped.");
    }
}