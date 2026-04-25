using UnityEngine;

public class SystemPerformanceManager : MonoBehaviour
{
    [Header("Performance Limits")]
    [SerializeField] private int _safeFileCount = 10;
    [SerializeField] private int _maxFileCount = 40;

    [SerializeField] private int _minFPS = 30;

    private void Start()
    {
        UpdateSystemPerformance();
    }

    private void OnEnable()
    {
        ActionCommands.OnNewFileCommand += UpdateSystemPerformance;
        ActionCommands.OnDeleteCommand += UpdateSystemPerformance;
        ActionCommands.OnFormatCommand += UpdateSystemPerformance;
        ActionCommands.OnRefreshCommand += UpdateSystemPerformance;
    }

    private void OnDisable()
    {
        ActionCommands.OnNewFileCommand -= UpdateSystemPerformance;
        ActionCommands.OnDeleteCommand -= UpdateSystemPerformance;
        ActionCommands.OnFormatCommand -= UpdateSystemPerformance;
        ActionCommands.OnRefreshCommand -= UpdateSystemPerformance;
    }

    private void UpdateSystemPerformance()
    {
        int currentFiles = FindObjectsByType<BaseFile>(FindObjectsInactive.Exclude).Length;

        if (currentFiles <= _safeFileCount)
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = -1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            float t = Mathf.Clamp01((float)(currentFiles - _safeFileCount) / (_maxFileCount - _safeFileCount));
            int targetFPS = Mathf.RoundToInt(Mathf.Lerp(60, _minFPS, t));

            Application.targetFrameRate = targetFPS;
            Debug.Log($"[WebGL Performance] Throttling to {targetFPS} FPS | Files: {currentFiles}");
        }
    }

}