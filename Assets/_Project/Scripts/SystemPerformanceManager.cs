using UnityEngine;

public class SystemPerformanceManager : MonoBehaviour
{
    [Header("Performance Limits")]
    [SerializeField] private int _safeFileCount = 10;
    [SerializeField] private int _maxFileCount = 40;

    [Header("FPS Settings")]
    [SerializeField] private int _maxFPS = 360;
    [SerializeField] private int _minFPS = 30;

    private int _currentFileCount;

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        _currentFileCount = FindObjectsByType<BaseFile>().Length;
        ApplyTargetFPS();
    }

    private void OnEnable()
    {
        ActionCommands.OnNewFileCommand += OnFileAdded;
        ActionCommands.OnDeleteCommand += OnFileRemoved;
        ActionCommands.OnFormatCommand += OnFileChanged;
    }

    private void OnDisable()
    {
        ActionCommands.OnNewFileCommand -= OnFileAdded;
        ActionCommands.OnDeleteCommand -= OnFileRemoved;
        ActionCommands.OnFormatCommand -= OnFileChanged;
    }

    // ─── Event Handlers ───────────────────────────────────────────────────────

    private void OnFileAdded()
    {
        _currentFileCount++;
        ApplyTargetFPS();
    }

    private void OnFileRemoved()
    {
        _currentFileCount = Mathf.Max(0, _currentFileCount - 1);
        ApplyTargetFPS();
    }

    private void OnFileChanged()
    {
        _currentFileCount = FindObjectsByType<BaseFile>(FindObjectsInactive.Exclude).Length;
        ApplyTargetFPS();
    }

    // ─── Core Logic ───────────────────────────────────────────────────────────

    private void ApplyTargetFPS()
    {
        int targetFPS;

        if (_currentFileCount <= _safeFileCount)
        {
            targetFPS = _maxFPS;
        }
        else if (_currentFileCount >= _maxFileCount)
        {
            targetFPS = _minFPS;
        }
        else
        {
            float t = (float)(_currentFileCount - _safeFileCount)
                            / (_maxFileCount - _safeFileCount);

            targetFPS = Mathf.RoundToInt(Mathf.Lerp(_maxFPS, _minFPS, t));
        }

        Application.targetFrameRate = targetFPS;

        Debug.Log($"[Performance] Files: {_currentFileCount} | Target FPS: {targetFPS}");
    }
}