using TMPro;
using UnityEngine;

public class BaseFile : MonoBehaviour
{
    [Header("File Status")]
    [SerializeField] protected int minSteps = 3;
    [SerializeField] protected int maxSteps = 7;

    protected int currentMaxSteps;
    public int curloadSteps { get; protected set; }

    [Header("File Name")]
    protected string brokenFileName;
    public string loadedFileName = "NewFile.exe";


    [Header("Name Generation")]
    private readonly string[] extensions = { ".tmp", ".rov", ".dll", ".log", ".dat" };

    [Header("UI Settings")]
    [SerializeField] protected TextMeshPro fileNameTextMeshPro;
    [SerializeField] protected TextMeshPro curloadStepsTextMeshProUI;

    [Header("Sprite Settings")]
    [SerializeField] protected Sprite baseSprites;
    [SerializeField] protected Sprite revealedSprites;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    private Vector3 offset;

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        GenerateComplexBrokenName();
        GenerateRandomLoadSteps();
        spriteRenderer.sprite = baseSprites;
        LoadFile();
    }

    protected void OnEnable()
    {
        ActionCommands.OnRefreshCommand += ReduceloadSteps;
    }

    protected virtual void OnDisable()
    {
        ActionCommands.OnRefreshCommand -= ReduceloadSteps;
    }

    protected virtual void ReduceloadSteps()
    {
        if (curloadSteps == 0)
        {
            GenerateRandomLoadSteps();
            GenerateComplexBrokenName();
        }
        else
        {
            curloadSteps--;
        }

        LoadFile();
    }

    private void GenerateRandomLoadSteps()
    {
        currentMaxSteps = Random.Range(minSteps, maxSteps + 1);
        curloadSteps = currentMaxSteps;
    }


    protected virtual void GenerateComplexBrokenName()
    {
        string hexCode = "0x" + Random.Range(0x0100, 0xFFFF).ToString("X");
        string ext = extensions[Random.Range(0, extensions.Length)];

        brokenFileName = hexCode + ext;
    }

    protected virtual void LoadFile()
    {
        if (curloadSteps == 0)
        {
            spriteRenderer.sprite = revealedSprites;

            if (fileNameTextMeshPro != null)
            {
                fileNameTextMeshPro.text = loadedFileName;
            }

        }
        else
        {
            spriteRenderer.sprite = baseSprites;
            if (fileNameTextMeshPro != null)
            {
                fileNameTextMeshPro.text = $"{brokenFileName}";
            }
        }

        if (curloadStepsTextMeshProUI != null)
        {
            curloadStepsTextMeshProUI.text = $"{curloadSteps}";

        }
    }

    private void OnMouseDown()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y, transform.position.z);
    }

    private void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, transform.position.z);
    }

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