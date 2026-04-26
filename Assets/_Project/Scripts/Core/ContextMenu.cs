using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ContextMenu : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject contextMenuPanel;
    public GameObject[] defaultMenuOptions;
    public GameObject emptyBinButton;
    public GameObject formatButton;

    [Header("Refresh System")]
    public int maxreFreshCount = 50;
    public int refreshCharges = 10;
    public int bonusPerNewFile = 8;

    [Header("Cursor UI")]
    public TextMeshProUGUI refreshChargesText;
    public Vector3 textOffset = new Vector3(20f, -20f, 0f);

    private BinFolder _targetBin;
    private CoreFolder _targetCoreFolder;
    private BaseFile hoveredFile;
    public static Action OnContextSelect;
    [SerializeField] private ScreenMateMovement _screenMateMovement;


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

        if (refreshChargesText != null)
        {
            refreshChargesText.text = refreshCharges.ToString();
            refreshChargesText.transform.position = Input.mousePosition + textOffset;

            if (refreshCharges <= 0)
            {
                refreshChargesText.color = Color.red;
            }
            else
            {
                refreshChargesText.color = Color.black;
            }
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
            if (defaultMenuOptions.Length > 1)
            {
                Button refreshBtn = defaultMenuOptions[1].GetComponent<Button>();
                if (refreshBtn != null)
                {
                    refreshBtn.interactable = refreshCharges > 0;
                }

                Image newFileImage = defaultMenuOptions[0].GetComponent<Image>();
                if (newFileImage != null)
                {
                    newFileImage.DOKill();
                    defaultMenuOptions[0].transform.DOKill();

                    defaultMenuOptions[0].transform.localScale = Vector3.one;

                    if (refreshCharges <= 0)
                    {
                        newFileImage.color = Color.white;
                        defaultMenuOptions[0].transform.DOScale(1.05f, 0.5f)
                                    .SetLoops(-1, LoopType.Yoyo)
                                    .SetUpdate(true);
                    }
                    else
                    {
                        newFileImage.color = Color.white;
                    }
                }
            }
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
        _screenMateMovement.StateOverride(ScreenElementState.Normal);
        refreshCharges = Mathf.Min(refreshCharges + bonusPerNewFile, maxreFreshCount);
        ActionCommands.OnNewFileCommand?.Invoke();
        OnContextSelect?.Invoke();
        CloseMenu();
        AudioManager.Instance.PlaySFX("Click");
    }

    public void OnClick_RefreshCommand()
    {
        _screenMateMovement.StateOverride(ScreenElementState.Normal);
        if (refreshCharges > 0)
        {
            refreshCharges--;
            ActionCommands.OnRefreshCommand?.Invoke();
            OnContextSelect?.Invoke();
            CloseMenu();
            AudioManager.Instance.PlaySFX("Refresh");
        }
    }

    public void OnClick_DeleteCommand()
    {
        if (hoveredFile != null)
        {
            if (hoveredFile.CanDelete(out string message))
            {
                hoveredFile.Delete();
                AudioManager.Instance.PlaySFX("Delete");
            }
            else
            {
                Debug.Log(message);
                AudioManager.Instance.PlaySFX("CantDelete");
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
            AudioManager.Instance.PlaySFX("EmptyBin");
        }
    }

    public void Execute_FormatCommand()
    {
        ActionCommands.OnFormatCommand?.Invoke();
        CloseMenu();
    }
}