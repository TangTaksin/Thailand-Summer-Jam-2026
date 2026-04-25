using TMPro;
using UnityEngine;

public class BaseFile : ScreenElements, IRefreshable, IDeletable
{
    [Header("File Status")]
    // 💡 1. เพิ่มตัวแปรสำหรับเปิด/ปิดการรับคำสั่ง Refresh
    [SerializeField] protected bool ignoreRefreshCommand = false; 
    
    [SerializeField] protected int minLoadSteps = 3;
    [SerializeField] protected int maxLoadSteps = 7;

    protected int _currentMaxLoadSteps;
    public int CurLoadSteps { get; protected set; }

    [Header("File Name")]
    protected string brokenFileName;
    [SerializeField] protected string _loadedFileName = "NewFile.exe";
    public string LoadedFileName => _loadedFileName;


    [Header("Name Generation")]
    private readonly string[] extensions = { ".tmp", ".rov", ".dll", ".log", ".dat" };

    [Header("UI Settings")]
    [SerializeField] protected TextMeshPro fileNameTextMeshPro;
    [SerializeField] protected TextMeshPro curloadStepsTextMeshProUI;

    [Header("Sprite Settings")]
    [SerializeField] protected Sprite baseSprites;
    [SerializeField] protected Sprite revealedSprites;
    protected SpriteRenderer spriteRenderer;
    protected Animator _animator;

    private Vector3 _dragOffset;
    private Camera _mainCamera;

    protected override void Start()
    {
        base.Start();
        _mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        GenerateComplexBrokenName();
        GenerateRandomLoadSteps();
        spriteRenderer.sprite = baseSprites;
        LoadFile();
    }

    protected virtual void OnEnable()
    {
        ActionCommands.OnRefreshCommand += Refresh;
    }

    protected virtual void OnDisable()
    {
        ActionCommands.OnRefreshCommand -= Refresh;
    }

    public virtual void Refresh()
    {
        // 💡 2. ดักไว้ตรงนี้ ถ้าตั้งค่าให้เมิน (true) ก็จะเด้งออกจากฟังก์ชันไปเลย ไม่ลดค่า Load Steps
        if (ignoreRefreshCommand) return; 
        
        ReduceloadSteps();
    }

    public virtual bool CanDelete(out string reason)
    {
        reason = "";
        return true;
    }

    public virtual void Delete()
    {
        Destroy(gameObject);
    }

    private void ReduceloadSteps()
    {
        if (CurLoadSteps == 0)
        {
            GenerateRandomLoadSteps();
            GenerateComplexBrokenName();
        }
        else
        {
            CurLoadSteps--;
        }

        LoadFile();
    }

    private void GenerateRandomLoadSteps()
    {
        _currentMaxLoadSteps = Random.Range(minLoadSteps, maxLoadSteps + 1);
        CurLoadSteps = _currentMaxLoadSteps;
    }


    protected virtual void GenerateComplexBrokenName()
    {
        string hexCode = "0x" + Random.Range(0x0100, 0xFFFF).ToString("X");
        string ext = extensions[Random.Range(0, extensions.Length)];

        brokenFileName = hexCode + ext;
    }

    protected virtual void LoadFile()
    {
        if (CurLoadSteps == 0)
        {
            spriteRenderer.sprite = revealedSprites;

            if (fileNameTextMeshPro != null)
            {
                fileNameTextMeshPro.text = _loadedFileName;
            }

            if (_animator != null) _animator.enabled = true;

        }
        else
        {
            spriteRenderer.sprite = baseSprites;
            if (fileNameTextMeshPro != null)
            {
                fileNameTextMeshPro.text = brokenFileName;
            }
            if (_animator != null) _animator.enabled = false;
        }

        if (curloadStepsTextMeshProUI != null)
        {
            curloadStepsTextMeshProUI.text = CurLoadSteps.ToString();

        }
    }

    protected virtual void OnMouseDown()
    {
        // Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // _dragOffset = transform.position - new Vector3(mousePos.x, mousePos.y, transform.position.z);
    }

    // protected virtual void OnMouseDrag()
    // {
    //     Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    //     transform.position = new Vector3(
    //         mousePos.x + _dragOffset.x,
    //         mousePos.y + _dragOffset.y,
    //         transform.position.z
    //     );
    // }

    protected virtual void OnMouseUp()
    {

        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);

        foreach (var hit in hits)
        {
            BaseFolder folder = hit.GetComponent<BaseFolder>();

            if (folder != null)
            {
                folder.ReceiveFile(this);
                break;
            }
        }
    }
}