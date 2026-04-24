using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TipManualUI : MonoBehaviour
{
    [System.Serializable]
    public struct ManualPage
    {
        public string title;
        [TextArea(3, 10)] public string description;
        public Sprite image;
    }

    [Header("UI References")]
    public GameObject panelObject;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image displayImage;
    public TextMeshProUGUI pageNumberText;

    [Header("Content")]
    public ManualPage[] pages;
    private int _currentPage = 0;

    void Awake()
    {
        if(panelObject != null) panelObject.SetActive(false);
    }

    public void OpenManual()
    {
        _currentPage = 0;
        UpdateUI();
        panelObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseManual()
    {
        panelObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void NextPage() => ChangePage(1);
    public void PreviousPage() => ChangePage(-1);

    private void ChangePage(int direction)
    {
        _currentPage = Mathf.Clamp(_currentPage + direction, 0, pages.Length - 1);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (pages == null || pages.Length == 0) return;

        titleText.text = pages[_currentPage].title;
        descriptionText.text = pages[_currentPage].description;
        displayImage.sprite = pages[_currentPage].image;
        pageNumberText.text = $"{_currentPage + 1} / {pages.Length}";
    }
}