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

    [Header("Animation Sprites")]
    public Sprite idleSprite;           // ภาพตอนยืนเฉยๆ (1 frame)
    public Sprite[] grabSprites;        // ภาพตอนโดนจับ (3 frames)
    public Sprite[] walkSprites;        // ภาพตอนเดิน (4 frames)
    public float animFrameRate = 0.15f; // ความเร็วในการเปลี่ยนเฟรม (วินาทีต่อเฟรม)

    // ตัวแปรสำหรับคุม Animation
    private float _animTimer = 0f;
    private int _currentAnimFrame = 0;
    private Sprite[] _currentAnimArray; // เก็บว่าตอนนี้กำลังเล่น Array ไหนอยู่

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
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        rb2D.freezeRotation = true;
        SetWalkState();
    }

    // อัปเดตฟิสิกส์และการเปลี่ยน State
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

    // 💡 อัปเดต Animation ภาพกราฟิกใน Update (ทำงานลื่นไหลกว่า FixedUpdate)
    private void Update()
    {
        if (_isGameOver) return;

        // เช็คว่าโดนจับ/คลิกลากอยู่หรือเปล่า (สถานะไม่ใช่ Normal)
        if (element_state != ScreenElementState.Normal)
        {
            PlayAnimation(grabSprites); // เล่นอนิเมชันตอนโดนจับ
        }
        else
        {
            // ถ้าไม่โดนจับ ก็เล่นอนิเมชันตาม State ปัจจุบัน
            switch (currentState)
            {
                case MateState.Walk:
                    PlayAnimation(walkSprites);
                    break;
                case MateState.Idle:
                case MateState.Falling:
                    // โหมด Idle หรือ กำลังตก ให้ใช้ภาพเดียว
                    SetSingleSprite(idleSprite);
                    break;
            }
        }
    }

    // 💡 ฟังก์ชันจัดการแอนิเมชันแบบสลับภาพ
    private void PlayAnimation(Sprite[] animArray)
    {
        if (animArray == null || animArray.Length == 0) return;

        // ถ้าเปลี่ยนท่าทาง ให้รีเซ็ตเฟรมกลับไปที่ 0
        if (_currentAnimArray != animArray)
        {
            _currentAnimArray = animArray;
            _currentAnimFrame = 0;
            _animTimer = 0f;
            _spriteRenderer.sprite = animArray[0];
            return;
        }

        // จับเวลาเพื่อเปลี่ยนภาพ
        _animTimer += Time.deltaTime;
        if (_animTimer >= animFrameRate)
        {
            _animTimer = 0f;
            _currentAnimFrame++;

            // วนลูปกลับไปภาพแรกถ้าเล่นจบ Array
            if (_currentAnimFrame >= animArray.Length)
            {
                _currentAnimFrame = 0;
            }

            _spriteRenderer.sprite = animArray[_currentAnimFrame];
        }
    }

    // 💡 ฟังก์ชันสำหรับตั้งภาพนิ่ง (1 เฟรม)
    private void SetSingleSprite(Sprite singleSprite)
    {
        if (singleSprite == null) return;
        
        // เคลียร์ค่า Array เพื่อให้เวลาสลับกลับไปเดิน/โดนจับ มันจะได้เริ่มเฟรม 0 ใหม่
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
        SetSingleSprite(idleSprite); // ให้มันยืนนิ่งๆ ตอน Game Over
        Debug.Log("[ScreenMateMovement] Game Over — Movement stopped.");
    }
}