using TMPro;
using UnityEngine;

public class BaseFile : MonoBehaviour
{
    [Header("File Status")]
    [SerializeField] protected int maxloadSteps = 5;
    protected int curloadSteps;

    [Header("File Name")]
    private string brokenFileName;
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
        spriteRenderer.sprite = baseSprites;
        curloadSteps = maxloadSteps;
        LoadFile();
    }

    protected void OnEnable()
    {
        RefreshCommand.OnRefreshCommand += ReduceloadSteps;
    }

    protected virtual void OnDisable()
    {
        RefreshCommand.OnRefreshCommand -= ReduceloadSteps;
    }

    public void ReduceloadSteps()
    {
        if (curloadSteps == 0)
        {
            curloadSteps = maxloadSteps;
            GenerateComplexBrokenName();
        }
        else
        {
            curloadSteps--;
        }

        LoadFile();
    }

    private void GenerateComplexBrokenName()
    {
        string hexCode = "0x" + Random.Range(0x0100, 0xFFFF).ToString("X");
        string ext = extensions[Random.Range(0, extensions.Length)];

        brokenFileName = hexCode + ext;
    }

    public virtual void LoadFile()
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
}