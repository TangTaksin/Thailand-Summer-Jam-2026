using UnityEngine;
using TMPro;
using System.Collections;

public class FileSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private int _spawnCount = 1;

    [Header("Gacha System Settings")]
    [SerializeField] private GameObject _coreFilePrefab;
    [SerializeField] private FileSpawnTable _spawnTable;

    [Range(0f, 100f)]
    [SerializeField] private float _coreFileDropRate = 5f;
    [SerializeField] private int _guaranteedPityCount = 10;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 _minBounds = new Vector2(-7f, -3f);
    [SerializeField] private Vector2 _maxBounds = new Vector2(7f, 4f);

    [Header("Indirect Feedback")]
    [SerializeField] private SpriteRenderer _mateRenderer;
    [SerializeField] private Color _readyColor = new Color(0.5f, 1f, 0.5f);
    [SerializeField] private float _shakeIntensity = 0.05f;

    [Header("System Notification Text")]
    [SerializeField] private TextMeshPro _terminalText;
    private Coroutine _messageRoutine;

    private int _currentPityCounter;

    private void OnEnable()
    {
        ActionCommands.OnNewFileCommand += SpawnRandomFile;
        ActionCommands.OnFileEaten += HandleCoreFileDrop;
    }

    private void OnDisable()
    {
        ActionCommands.OnNewFileCommand -= SpawnRandomFile;
        ActionCommands.OnFileEaten -= HandleCoreFileDrop;
    }

    private void Update()
    {
        ApplyIndirectFeedback();
    }

    private void SpawnRandomFile()
    {
        if (_spawnTable == null) return;

        for (int i = 0; i < _spawnCount; i++)
        {
            GameObject prefabToSpawn = _spawnTable.Pick();
            if (prefabToSpawn == null) continue;

            SpawnAtPosition(prefabToSpawn, GetRandomSpawnPos());
        }
    }

    private void HandleCoreFileDrop(BaseFile eatenFile)
    {
        if (eatenFile is BadFile) return;

        _currentPityCounter++;

        bool isPityTriggered = _currentPityCounter >= _guaranteedPityCount;
        float roll = Random.Range(0f, 100f);

        if (isPityTriggered || roll <= _coreFileDropRate)
        {
            ResetPity();
            ShowMessage("[Success] Core.exe successfully extracted.");
            SpawnAtPosition(_coreFilePrefab, eatenFile.transform.position);
        }
        else
        {
            Debug.Log($"[Gacha] Pity Count: {_currentPityCounter}/{_guaranteedPityCount}");
        }
    }

    private void ShowMessage(string msg)
    {
        if (_terminalText == null) return;
        if (_messageRoutine != null)
            StopCoroutine(_messageRoutine);

        _messageRoutine = StartCoroutine(ShowMessageRoutine(msg, 5f));
    }

    private void ApplyIndirectFeedback()
    {
        if (_mateRenderer == null) return;

        float progress = (float)_currentPityCounter / _guaranteedPityCount;
        _mateRenderer.color = Color.Lerp(Color.white, _readyColor, progress);
        if (progress > 0.7f)
        {
            float currentShake = _shakeIntensity * progress;
            _mateRenderer.transform.localPosition += (Vector3)Random.insideUnitCircle * currentShake;
        }
    }

    private void SpawnAtPosition(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return;
        Instantiate(prefab, position, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPos()
    {
        return new Vector3(
            Random.Range(_minBounds.x, _maxBounds.x),
            Random.Range(_minBounds.y, _maxBounds.y),
            0f
        );
    }

    private void ResetPity() => _currentPityCounter = 0;

    private IEnumerator ShowMessageRoutine(string msg, float duration)
    {
        _terminalText.text = msg;
        _terminalText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        _terminalText.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Vector3 center = (Vector3)(_minBounds + _maxBounds) / 2f;
        Vector3 size = new Vector3(_maxBounds.x - _minBounds.x, _maxBounds.y - _minBounds.y, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}