using UnityEngine;

public class BadFile : BaseFile, IStatModifier, IMovable
{
    [Header("HP System")]
    [SerializeField] private int _maxHp = 3;
    [SerializeField] private int _currentHp;
    [SerializeField] private bool _isStunned = false;

    [Header("Combat Settings")]
    [SerializeField] public float damage = 20f;

    [Header("Movement Settings")]
    [field: SerializeField] public float MoveSpeed { get; set; } = 2f;

    private Transform _targetScreenMate;

    protected override void Start()
    {
        base.Start();
        InitializeHp();
        FindScreenMateTarget();
    }

    private void Update()
    {
        bool canMove = curloadSteps == 0 && !_isStunned && _targetScreenMate != null;

        if (canMove)
        {
            MoveToTarget(_targetScreenMate);
        }
    }

    private void InitializeHp()
    {
        _currentHp = _maxHp;
    }

    private void FindScreenMateTarget()
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

    protected override void ReduceloadSteps()
    {
        if (curloadSteps > 0)
        {
            HandleLoadStepReduction();
        }
        else
        {
            HandleCombatPhase();
        }

        LoadFile();
    }

    private void HandleLoadStepReduction()
    {
        curloadSteps--;

        bool finishedLoading = curloadSteps == 0;

        if (finishedLoading)
        {
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
        bool isLoadingOrStunned = curloadSteps > 0 || _isStunned;

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
                fileNameTextMeshPro.text = loadedFileName;
            }
        }
    }

    private void UpdateStatusText()
    {
        if (curloadStepsTextMeshProUI == null)
        {
            return;
        }

        if (curloadSteps > 0)
        {
            curloadStepsTextMeshProUI.text = $"{curloadSteps}";
        }
        else
        {
            curloadStepsTextMeshProUI.text = $"{_currentHp}";
        }
    }

    public void ApplyModifier(ScreenMateStats stats)
    {
        bool canDealDamage = curloadSteps == 0 && !_isStunned;

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