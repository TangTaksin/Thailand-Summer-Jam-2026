using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenu : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject contextMenuPanel;
    private BaseFile hoveredFile;

    private void Start()
    {
        if (contextMenuPanel != null)
        {
            contextMenuPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                CloseMenu();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            OpenMenu();
        }
    }

    private void OpenMenu()
    {
        RectTransform rect = contextMenuPanel.GetComponent<RectTransform>();

        rect.pivot = new Vector2(0f, 1f);

        Vector3 mouseOffset = new Vector3(5f, -5f, 0f);
        rect.position = Input.mousePosition + mouseOffset;

        contextMenuPanel.SetActive(true);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            hoveredFile = hit.collider.GetComponent<BaseFile>();
        }
        else
        {
            hoveredFile = null;
        }
    }

    private void CloseMenu()
    {
        contextMenuPanel.SetActive(false);
        hoveredFile = null;
    }

    public void OnClick_NewFileCommand()
    {
        ActionCommands.OnNewFileCommand?.Invoke();
        CloseMenu();
    }

    public void OnClick_RefreshCommand()
    {
        ActionCommands.OnRefreshCommand?.Invoke();
        CloseMenu();
    }

    public void OnClick_DeleteCommand()
    {
        if (hoveredFile != null)
        {
            if (hoveredFile.CanDelete(out string message))
            {
                Debug.Log("ลบสำเร็จ");
                hoveredFile.Delete();
            }
            else
            {
                Debug.Log(message);
            }
        }
        CloseMenu();
    }
}