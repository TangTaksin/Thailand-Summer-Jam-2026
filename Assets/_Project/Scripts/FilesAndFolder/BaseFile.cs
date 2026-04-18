using UnityEngine;

public class BaseFile : MonoBehaviour
{
    [Header("File Status")]
    [SerializeField] protected int maxloadSteps = 5;
    [SerializeField] protected int curloadSteps;
    [SerializeField] protected bool isLoaded = false;

    [Header("Sprite Settings")]
    [SerializeField] protected Sprite baseSprites;
    [SerializeField] protected Sprite revealedSprites;
    [SerializeField] protected SpriteRenderer spriteRenderer;


    private Vector3 offset;

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            isLoaded = false;
            LoadFile();
            return;
        }

        curloadSteps--;

        if (curloadSteps <= 0)
        {
            isLoaded = true;
            Debug.Log("File Loaded Successfully");
            LoadFile();
        }
    }

    public virtual void LoadFile()
    {
        if (curloadSteps == 0)
        {
            spriteRenderer.sprite = revealedSprites;
        }
        else
        {
            spriteRenderer.sprite = baseSprites;
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
