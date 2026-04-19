using UnityEngine;

/// <summary>
/// Spawns files using a Gacha/Pity system.
/// </summary>
public class FileSpawner : MonoBehaviour
{
    // ─────────────────────────────────────────
    // Inspector Fields
    // ─────────────────────────────────────────

    [Header("Gacha System Settings")]
    [Tooltip("Core File prefab")]
    [SerializeField] private GameObject _coreFilePrefab;

    [Tooltip("Regular file prefabs")]
    [SerializeField] private GameObject[] _regularFilePrefabs;

    [Tooltip("Drop chance for Core File (0–100%)")]
    [Range(0f, 100f)]
    [SerializeField] private float _coreFileDropRate = 5f;

    [Tooltip("Guaranteed Core File after N rolls")]
    [SerializeField] private int _guaranteedPityCount = 10;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 _minBounds = new Vector2(-7f, -3f);
    [SerializeField] private Vector2 _maxBounds = new Vector2(7f,  4f);

    // ─────────────────────────────────────────
    // Private State
    // ─────────────────────────────────────────

    private int _currentPityCounter;

    // ─────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────

    private void OnEnable()
    {
        ActionCommands.OnNewFileCommand += SpawnRandomFile;
    }

    private void OnDisable()
    {
        ActionCommands.OnNewFileCommand -= SpawnRandomFile;
    }

    // ─────────────────────────────────────────
    // Spawn Logic
    // ─────────────────────────────────────────

    private void SpawnRandomFile()
    {
        if (!ValidatePrefabs()) return;

        GameObject prefabToSpawn = ResolvePrefab();

        Vector3 spawnPosition = new Vector3(
            Random.Range(_minBounds.x, _maxBounds.x),
            Random.Range(_minBounds.y, _maxBounds.y),
            0f
        );

        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }

    /// <summary>
    /// Pure gacha logic — decides which prefab to spawn.
    /// Mutates _currentPityCounter as a side-effect.
    /// </summary>
    private GameObject ResolvePrefab()
    {
        _currentPityCounter++;

        // Pity guarantee
        if (_currentPityCounter >= _guaranteedPityCount)
        {
            ResetPity();
            Debug.Log("[FileSpawner] 🎉 Pity triggered — Core File guaranteed.");
            return _coreFilePrefab;
        }

        // Roll for SSR
        float roll = Random.Range(0f, 100f);
        if (roll <= _coreFileDropRate)
        {
            ResetPity();
            Debug.Log($"[FileSpawner] ✨ Lucky roll ({roll:F1}%) — Core File dropped!");
            return _coreFilePrefab;
        }

        // Regular file
        int index = Random.Range(0, _regularFilePrefabs.Length);
        Debug.Log($"[FileSpawner] 📄 Regular file (pity: {_currentPityCounter}/{_guaranteedPityCount})");
        return _regularFilePrefabs[index];
    }

    private void ResetPity() => _currentPityCounter = 0;

    // ─────────────────────────────────────────
    // Validation
    // ─────────────────────────────────────────

    private bool ValidatePrefabs()
    {
        if (_coreFilePrefab == null || _regularFilePrefabs == null || _regularFilePrefabs.Length == 0)
        {
            Debug.LogWarning("[FileSpawner] Prefabs not fully assigned in Inspector.");
            return false;
        }
        return true;
    }

    // ─────────────────────────────────────────
    // Editor Gizmos
    // ─────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Vector3 center = new Vector3(
            (_minBounds.x + _maxBounds.x) / 2f,
            (_minBounds.y + _maxBounds.y) / 2f,
            0f
        );
        Vector3 size = new Vector3(
            _maxBounds.x - _minBounds.x,
            _maxBounds.y - _minBounds.y,
            0f
        );
        Gizmos.DrawWireCube(center, size);
    }
}