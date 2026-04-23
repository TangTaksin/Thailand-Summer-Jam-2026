using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private ContextMenu contextMenu;

    [Header("UI Panels")]
    [SerializeField] private GameObject _gameOverPanel;
    private bool _isSceneLoading = false;

    void Start()
    {
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        ActionCommands.OnGameOver += HandleGameOver;
        ActionCommands.OnFormatCommand += HandleWin;
    }

    private void OnDisable()
    {
        ActionCommands.OnGameOver -= HandleGameOver;
        ActionCommands.OnFormatCommand -= HandleWin;
    }

    private void HandleWin()
    {
        if (_isSceneLoading) return;
        _isSceneLoading = true;
        DOTween.KillAll();
        LoadNextScene();
    }

    private void HandleGameOver()
    {
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
            DisableControls();
            Time.timeScale = 0f;
        }

    }

    private void DisableControls()
    {
        if (contextMenu != null)
        {
            contextMenu.enabled = false;
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("EndScene");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        _gameOverPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("StartMenu");

    }
}
