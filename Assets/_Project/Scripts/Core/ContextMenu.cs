using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject contextMenuPanel;
    public GameObject[] defaultMenuOptions;
    public GameObject emptyBinButton;
    public GameObject formatButton;

    private BinFolder _targetBin;
    private CoreFolder _targetCoreFolder;
    private BaseFile hoveredFile;

    public static Action OnContextSelect;

    private void Start()
    {
        if (contextMenuPanel != null)
        {
            contextMenuPanel.SetActive(false);
            emptyBinButton.SetActive(false);
            formatButton.SetActive(false);
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
        if (contextMenuPanel == null) return;


        RectTransform rect = contextMenuPanel.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0f, 1f);
        Vector3 mouseOffset = new Vector3(5f, -5f, 0f);
        rect.position = Input.mousePosition + mouseOffset;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        _targetBin = null;
        _targetCoreFolder = null;
        hoveredFile = null;

        if (hit.collider != null)
        {
            hoveredFile = hit.collider.GetComponent<BaseFile>();
            _targetBin = hit.collider.GetComponent<BinFolder>();
            _targetCoreFolder = hit.collider.GetComponent<CoreFolder>();
        }

        UpdateMenuOptions();
        contextMenuPanel.SetActive(true);
    }

    private void UpdateMenuOptions()
    {
        if (emptyBinButton != null) emptyBinButton.SetActive(false);
        if (formatButton != null) formatButton.SetActive(false);

        if (_targetBin != null)
        {
            emptyBinButton.SetActive(true);
            ToggleDefaultOptions(false);

            Button btn = emptyBinButton.GetComponent<Button>();
            if (btn != null)
            {
                btn.interactable = _targetBin.CanEmpty;
            }
        }
        else if (_targetCoreFolder != null)
        {
            formatButton.SetActive(true);
            ToggleDefaultOptions(false);

            Button btn = formatButton.GetComponent<Button>();
            if (btn != null)
            {
                btn.interactable = _targetCoreFolder.TargetCount == 0;
            }
        }
        else
        {
            ToggleDefaultOptions(true);
        }
    }

    private void ToggleDefaultOptions(bool isActive)
    {
        foreach (GameObject option in defaultMenuOptions)
        {
            if (option != null) option.SetActive(isActive);
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
        OnContextSelect?.Invoke();
        CloseMenu();
    }

    public void OnClick_RefreshCommand()
    {
        ActionCommands.OnRefreshCommand?.Invoke();
        OnContextSelect?.Invoke();
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
        ActionCommands.OnDeleteCommand?.Invoke();
        OnContextSelect?.Invoke();
        CloseMenu();
    }

    public void Execute_EmptyBinCommand()
    {
        if (_targetBin != null && _targetBin.CanEmpty)
        {
            ActionCommands.OnEmptyBinCommand?.Invoke();
            CloseMenu();
        }
    }

    public void Execute_FormatCommand()
    {
        ActionCommands.OnFormatCommand?.Invoke();
        CloseMenu();
    }
}