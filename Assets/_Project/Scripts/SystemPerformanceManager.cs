using UnityEngine;

public class SystemPerformanceManager : MonoBehaviour
{
    [Header("Performance Limits")]
    [SerializeField] private int _safeFileCount = 10;
    [SerializeField] private int _maxFileCount = 40;
    
    [Header("FPS Settings")]
    [SerializeField] private int _maxFPS = 360;
    [SerializeField] private int _minFPS = 30;

    private void Start()
    {
        QualitySettings.vSyncCount = 0; 
        UpdateSystemPerformance();
    }

    private void OnEnable()
    {
        ActionCommands.OnNewFileCommand += UpdateSystemPerformance;
        ActionCommands.OnDeleteCommand += UpdateSystemPerformance;
        ActionCommands.OnFormatCommand += UpdateSystemPerformance;
    }

    private void OnDisable()
    {
        ActionCommands.OnNewFileCommand -= UpdateSystemPerformance;
        ActionCommands.OnDeleteCommand -= UpdateSystemPerformance;
        ActionCommands.OnFormatCommand -= UpdateSystemPerformance;
    }

    private void UpdateSystemPerformance()
    {
        int currentFiles = FindObjectsByType<BaseFile>(FindObjectsInactive.Exclude).Length;

        if (currentFiles <= _safeFileCount)
        {
            Application.targetFrameRate = _maxFPS;
        }
        else if (currentFiles >= _maxFileCount)
        {
            Application.targetFrameRate = _minFPS;
        }
        else
        {
            float t = (float)(currentFiles - _safeFileCount) / (_maxFileCount - _safeFileCount);
            int targetFPS = Mathf.RoundToInt(Mathf.Lerp(_maxFPS, _minFPS, t));
            
            Application.targetFrameRate = targetFPS;
        }

        Debug.Log($"[Performance] Files: {currentFiles} | Target FPS: {Application.targetFrameRate}");
    }
}