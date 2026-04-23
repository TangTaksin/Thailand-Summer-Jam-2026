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

    [Header("Animation Sprites (Normal)")]
    public Sprite idleSprite;           // ภาพตอนยืนเฉยๆ ปกติ
    public Sprite[] grabSprites;        // ภาพตอนโดนจับ ปกติ
    public Sprite[] walkSprites;        // ภาพตอนเดิน ปกติ
    public float animFrameRate = 0.15f; 

    // 💡 เพิ่มชุดภาพสำหรับโหมดอมตะ
    [Header("Animation Sprites (Invincible)")]
    public Sprite invincibleIdleSprite;    // ภาพตอนยืนเฉยๆ โหมดอมตะ
    public Sprite[] invincibleGrabSprites; // ภาพตอนโดนจับ โหมดอมตะ
    public Sprite[] invincibleWalkSprites; // ภาพตอนเดิน โหมดอมตะ

    // ตัวแปรสำหรับคุม Animation
    private float _animTimer = 0f;
    private int _currentAnimFrame = 0;
    private Sprite[] _currentAnimArray; 

    private int movingDirection = 1;
    private float stateTimer = 0f;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private bool _isGameOver = false;

    // 💡 ตัวแปรเชื่อมต่อไปยังสคริปต์ Stats เพื่อเช็คสถานะอมตะ
    private ScreenMateStats _stats;

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
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 💡 ค้นหาสคริปต์ ScreenMateStats ที่อยู่ในตัวละครนี้
        _stats = GetComponent<ScreenMateStats>(); 
        
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

    private void Update()
    {
        if (_isGameOver) return;

        // 💡 เช็คว่าตอนนี้อยู่ในโหมดอมตะหรือไม่
        bool isInvincible = (_stats != null && _stats._isInvincible);

        if (element_state != ScreenElementState.Normal)
        {
            // ถ้าเป็นอมตะ ให้เล่นภาพ invincibleGrab ถ้าไม่ ให้เล่น grab ปกติ
            PlayAnimation(isInvincible ? invincibleGrabSprites : grabSprites);
        }
        else
        {
            switch (currentState)
            {
                case MateState.Walk:
                    PlayAnimation(isInvincible ? invincibleWalkSprites : walkSprites);
                    break;
                case MateState.Idle:
                case MateState.Falling:
                    SetSingleSprite(isInvincible ? invincibleIdleSprite : idleSprite);
                    break;
            }
        }
    }

    private void PlayAnimation(Sprite[] animArray)
    {
        // 💡 ป้องกัน Error ถ้าลืมใส่ภาพใน Inspector
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
        if (_animTimer >= animFrameRate)
        {
            _animTimer = 0f;
            _currentAnimFrame++;

            if (_currentAnimFrame >= animArray.Length)
            {
                _currentAnimFrame = 0;
            }

            _spriteRenderer.sprite = animArray[_currentAnimFrame];
        }
    }

    private void SetSingleSprite(Sprite singleSprite)
    {
        if (singleSprite == null) return;
        
        _currentAnimArray = null; 
        _spriteRenderer.sprite = singleSprite;
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

    private void HandleGameOver()
    {
        _isGameOver = true;
        rb2D.linearVelocity = Vector2.zero;
        currentState = MateState.Idle;
        
        // 💡 เช็คสถานะตอนตายด้วยว่าเป็นอมตะอยู่หรือเปล่า
        bool isInvincible = (_stats != null && _stats._isInvincible);
        SetSingleSprite(isInvincible ? invincibleIdleSprite : idleSprite); 
        
        Debug.Log("[ScreenMateMovement] Game Over — Movement stopped.");
    }
}