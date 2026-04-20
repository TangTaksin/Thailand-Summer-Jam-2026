using UnityEngine;

public class FileSpawner : MonoBehaviour
{
    [Header("Gacha System Settings")]
    [SerializeField] private GameObject _coreFilePrefab;
    [SerializeField] private FileSpawnTable _spawnTable;

    [Range(0f, 100f)]
    [SerializeField] private float _coreFileDropRate = 5f;
    [SerializeField] private int _guaranteedPityCount = 10;

    [Header("Spawn Settings")]
    [SerializeField] private int _spawnCount = 1;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 _minBounds = new Vector2(-7f, -3f);
    [SerializeField] private Vector2 _maxBounds = new Vector2(7f, 4f);

    private int _currentPityCounter;

    private void OnEnable() => ActionCommands.OnNewFileCommand += SpawnRandomFile;
    private void OnDisable() => ActionCommands.OnNewFileCommand -= SpawnRandomFile;

    private void SpawnRandomFile()
    {
        if (_coreFilePrefab == null || _spawnTable == null)
        {
            Debug.LogWarning("[FileSpawner] Prefab หรือ SpawnTable ยังไม่ได้ assign");
            return;
        }

        for (int i = 0; i < _spawnCount; i++)
        {
            GameObject prefabToSpawn = ResolvePrefab();
            if (prefabToSpawn == null) continue;

            Vector3 spawnPos = new Vector3(
                Random.Range(_minBounds.x, _maxBounds.x),
                Random.Range(_minBounds.y, _maxBounds.y),
                0f
            );

            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        }
    }

    private GameObject ResolvePrefab()
    {
        _currentPityCounter++;

        if (_currentPityCounter >= _guaranteedPityCount)
        {
            ResetPity();
            Debug.Log("[FileSpawner] 🎉 Pity triggered!");
            return _coreFilePrefab;
        }

        float roll = Random.Range(0f, 100f);
        if (roll <= _coreFileDropRate)
        {
            ResetPity();
            Debug.Log($"[FileSpawner] ✨ Lucky roll ({roll:F1}%)!");
            return _coreFilePrefab;
        }

        return _spawnTable.Pick();
    }

    private void ResetPity() => _currentPityCounter = 0;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Vector3 center = new Vector3(
            (_minBounds.x + _maxBounds.x) / 2f,
            (_minBounds.y + _maxBounds.y) / 2f, 0f
        );
        Vector3 size = new Vector3(
            _maxBounds.x - _minBounds.x,
            _maxBounds.y - _minBounds.y, 0f
        );
        Gizmos.DrawWireCube(center, size);
    }
}