using UnityEngine;

public class BadFile : BaseFile, IStatModifier, IMovable
{
    [Header("HP System")]
    [SerializeField] private Vector2Int _hpRange = new Vector2Int(2, 5);
    [SerializeField] private int _maxHp;
    [SerializeField] private int _currentHp;
    public int CurrentHp => _currentHp;
    [SerializeField] private bool _isStunned = false;

    [Header("Combat Settings")]
    [SerializeField] public float damage = 20f;

    [Header("Spawn Mode")]
    [SerializeField] protected bool _startRevealed = false;

    [Header("Movement Settings")]
    [field: SerializeField] public float MoveSpeed { get; set; } = 2f;

    protected Transform _targetScreenMate;

    protected override void Start()
    {
        base.Start();
        _maxHp = Random.Range(_hpRange.x, _hpRange.y + 1);
        InitializeHp();
        FindScreenMateTarget();

        if (_startRevealed)
        {
            CurLoadSteps = 0;
            InitializeHp();
        }

        LoadFile();

    }

    private void Update()
    {
        if (element_state != ScreenElementState.Normal) return;
        
        bool canMove = CurLoadSteps == 0 && !_isStunned && _targetScreenMate != null;

        if (canMove)
        {
            MoveToTarget(_targetScreenMate);
        }
    }

    protected void InitializeHp()
    {
        _currentHp = _maxHp;
    }

    protected void FindScreenMateTarget()
    {
        ScreenMateStats screenMate = FindAnyObjectByType<ScreenMateStats>();

        if (screenMate != null)
        {
            _targetScreenMate = screenMate.transform;
        }
        else
        {
            Debug.LogWarning($"[BadFile] ไม่พบ ScreenMateStats ใน Scene — {gameObject.name} จะไม่มีเป้าหมาย");
        }
    }

    public override void Refresh()
    {
        if (CurLoadSteps > 0)
        {
            HandleLoadStepReduction();
        }
        else
        {

            HandleCombatPhase();
        }

        LoadFile();
    }

    public override bool CanDelete(out string reason)
    {
        if (CurrentHp > 0)
        {
            reason = "ไม่สามารถลบไวรัสได้! ต้องทำให้มัน Stun ก่อน";
            return false;
        }

        reason = "";
        return true;
    }

    private void HandleLoadStepReduction()
    {
        CurLoadSteps--;

        bool finishedLoading = CurLoadSteps <= 0;

        if (finishedLoading)
        {
            CurLoadSteps = 0;
            InitializeHp();
            _isStunned = false;
            Debug.Log($"[BadFile] {gameObject.name} โหลดเสร็จแล้ว — พร้อมเคลื่อนที่");
        }
    }

    private void HandleCombatPhase()
    {
        if (!_isStunned)
        {
            TakeDamage();
        }
        else
        {
            RecoverFromStun();
        }
    }

    private void TakeDamage()
    {
        _currentHp--;

        bool isDefeated = _currentHp <= 0;

        if (isDefeated)
        {
            _currentHp = 0;
            _isStunned = true;
            Debug.Log($"[BadFile] {gameObject.name} ถูกทำลาย — เข้าสู่สถานะ Stunned");
        }
    }

    private void RecoverFromStun()
    {
        _currentHp = _maxHp;
        _isStunned = false;
        Debug.Log($"[BadFile] {gameObject.name} ฟื้นฟูแล้ว — กลับมา HP เต็ม");
    }

    protected override void LoadFile()
    {
        UpdateSpriteAndFileName();
        UpdateStatusText();
    }

    private void UpdateSpriteAndFileName()
    {
        bool isLoadingOrStunned = CurLoadSteps > 0 || _isStunned;

        if (isLoadingOrStunned)
        {
            if (fileNameTextMeshPro != null)
            {
                if (_isStunned)
                {
                    fileNameTextMeshPro.text = "FILE_STUNNED.tmp";
                }
                else
                {
                    fileNameTextMeshPro.text = brokenFileName;
                }
            }
        }
        else
        {
            spriteRenderer.sprite = revealedSprites;

            if (fileNameTextMeshPro != null)
            {
                fileNameTextMeshPro.text = _loadedFileName;
            }
        }
    }

    private void UpdateStatusText()
    {
        if (curloadStepsTextMeshProUI == null)
        {
            return;
        }

        if (CurLoadSteps > 0)
        {
            curloadStepsTextMeshProUI.text = $"{CurLoadSteps}";
        }
        else
        {
            curloadStepsTextMeshProUI.text = $"{_currentHp}";
        }
    }

    public void ApplyModifier(ScreenMateStats stats)
    {
        bool canDealDamage = CurLoadSteps == 0 && !_isStunned;

        if (!canDealDamage)
        {
            return;
        }

        stats.UpdateCortisol(damage);
        Debug.Log($"[BadFile] {gameObject.name} โจมตี ScreenMate ด้วย Cortisol {damage}");
        Destroy(gameObject);
    }

    public void MoveToTarget(Transform target)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            MoveSpeed * Time.deltaTime
        );
    }
}