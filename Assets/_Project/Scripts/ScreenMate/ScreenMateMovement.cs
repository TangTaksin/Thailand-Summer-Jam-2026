using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class ScreenMateMovement : ScreenElements
{
    public enum MateState { Walk, Idle, Falling }

    [System.Serializable]
    public struct SpriteSet
    {
        public Sprite idleSprite;
        public Sprite[] grabSprites;
        public Sprite[] walkSprites;
    }

    [Header("Current State")]
    [SerializeField] private MateState _currentState = MateState.Walk;
    public MateState CurrentState => _currentState;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _leftBound = -7f;
    [SerializeField] private float _rightBound = 7f;

    [Header("State Settings")]
    [SerializeField] private float _minWalkTime = 2f;
    [SerializeField] private float _maxWalkTime = 6f;
    [SerializeField] private float _minIdleTime = 1f;
    [SerializeField] private float _maxIdleTime = 3f;

    [Header("Animation Settings")]
    [SerializeField] private float _animFrameRate = 0.15f;
    [SerializeField] private SpriteSet _normalSprites;
    [SerializeField] private SpriteSet _invincibleSprites;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckDistance = 0.1f;
    [SerializeField] private float _shellRadius = 0.05f;

  
    private SpriteRenderer _spriteRenderer;
    private ScreenMateStats _stats;

  
    private float _animTimer = 0f;
    private int _currentAnimFrame = 0;
    private Sprite[] _currentAnimArray;
    private int _movingDirection = 1;
    private float _stateTimer = 0f;
    private bool _isGameOver = false;
    private bool _isGrounded;

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
        _stats = GetComponent<ScreenMateStats>();
        
        if (rb2D != null) rb2D.freezeRotation = true;
        
        SetWalkState();
    }

    private void FixedUpdate()
    {
        if (_isGameOver || element_state != ScreenElementState.Normal) return;

        CheckGround();
        UpdateStateByPhysics();

        switch (_currentState)
        {
            case MateState.Walk:
                HandleStateTimer();
                Patrol();
                break;

            case MateState.Idle:
            case MateState.Falling:
                HandleStateTimer();
                StopHorizontalMovement();
                break;
        }
    }

    private void Update()
    {
        if (_isGameOver) return;
        UpdateAnimation();
    }

    #region Movement & State Logic

    private void CheckGround()
    {
        if (_collider == null) return;

        Vector2 origin = new Vector2(_collider.bounds.center.x, _collider.bounds.min.y + _shellRadius);
        float distance = _groundCheckDistance + _shellRadius;

        _isGrounded = Physics2D.Raycast(origin, Vector2.down, distance, _groundLayer);

        Debug.DrawRay(origin, Vector2.down * distance, _isGrounded ? Color.green : Color.red);
    }

    private void UpdateStateByPhysics()
    {
        if (rb2D == null) return;

        if (!_isGrounded && rb2D.linearVelocity.y < -0.1f)
        {
            _currentState = MateState.Falling;
        }
        else if (_isGrounded && _currentState == MateState.Falling)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 0f);
            SetWalkState();
        }
    }

    private void HandleStateTimer()
    {
        if (_currentState == MateState.Falling) return;

        _stateTimer -= Time.fixedDeltaTime;
        if (_stateTimer <= 0)
        {
            if (_currentState == MateState.Idle) SetWalkState();
            else if (_currentState == MateState.Walk) SetIdleState();
        }
    }

    private void SetWalkState()
    {
        _currentState = MateState.Walk;
        _stateTimer = Random.Range(_minWalkTime, _maxWalkTime);
    }

    private void SetIdleState()
    {
        _currentState = MateState.Idle;
        _stateTimer = Random.Range(_minIdleTime, _maxIdleTime);
    }

    private void Patrol()
    {
        if (rb2D == null) return;

        rb2D.linearVelocity = new Vector2(_movingDirection * _moveSpeed, rb2D.linearVelocity.y);

        if (transform.position.x >= _rightBound && _movingDirection == 1)
        {
            SetDirection(-1);
        }
        else if (transform.position.x <= _leftBound && _movingDirection == -1)
        {
            SetDirection(1);
        }
    }

    private void SetDirection(int dir)
    {
        _movingDirection = dir;
        _spriteRenderer.flipX = (_movingDirection == -1);
    }

    private void StopHorizontalMovement()
    {
        if (rb2D != null)
            rb2D.linearVelocity = new Vector2(0, rb2D.linearVelocity.y);
    }

    #endregion

    #region Animation Logic

    private void UpdateAnimation()
    {
        bool isInvincible = (_stats != null && _stats._isInvincible);
        SpriteSet currentSet = isInvincible ? _invincibleSprites : _normalSprites;

        if (element_state != ScreenElementState.Normal)
        {
            PlayAnimation(currentSet.grabSprites);
            return;
        }

        switch (_currentState)
        {
            case MateState.Walk:
                PlayAnimation(currentSet.walkSprites);
                break;
            case MateState.Idle:
            case MateState.Falling:
                SetSingleSprite(currentSet.idleSprite);
                break;
        }
    }

    private void PlayAnimation(Sprite[] animArray)
    {
        if (animArray == null || animArray.Length == 0) return;

        if (_currentAnimArray != animArray)
        {
            _currentAnimArray = animArray;
            _currentAnimFrame = 0;
            _animTimer = 0f;
            _spriteRenderer.sprite = animArray[0];
            return;
        }

        _animTimer += Time.deltaTime;
        if (_animTimer >= _animFrameRate)
        {
            _animTimer = 0f;
            _currentAnimFrame = (_currentAnimFrame + 1) % animArray.Length;
            _spriteRenderer.sprite = animArray[_currentAnimFrame];
        }
    }

    private void SetSingleSprite(Sprite singleSprite)
    {
        if (singleSprite == null) return;
        _currentAnimArray = null;
        _spriteRenderer.sprite = singleSprite;
    }
    #endregion

    private void HandleGameOver()
    {
        _isGameOver = true;
        StopHorizontalMovement();
        _currentState = MateState.Idle;

        bool isInvincible = (_stats != null && _stats._isInvincible);
        SpriteSet finalSet = isInvincible ? _invincibleSprites : _normalSprites;

        SetSingleSprite(finalSet.idleSprite);
        Debug.Log($"<color=red>[{name}]</color> Game Over - Movement stopped.");
    }
}