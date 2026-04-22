using UnityEngine;

public class DebugSpawner : MonoBehaviour
{
    // -----------------------------------------------------------------------
    // Serializable entry — pairs a key, prefab, and label together
    // so the Inspector shows everything in one place.
    // -----------------------------------------------------------------------
    [System.Serializable]
    public struct SpawnEntry
    {
        [Tooltip("กดปุ่มนี้เพื่อ Spawn")]
        public KeyCode key;

        [Tooltip("Prefab ที่จะถูก Spawn")]
        public GameObject prefab;

        [Tooltip("ชื่อที่แสดงใน Console")]
        public string label;
    }

    [Header("── Spawn Entries ──────────────────")]
    [Tooltip("เพิ่ม/ลด entry ได้เลย ไม่ต้องแก้ Code")]
    [SerializeField] private SpawnEntry[] _spawnEntries;

    [Header("── Spawn Position ──────────────────")]
    [Tooltip("ON = Spawn ที่เมาส์  |  OFF = Spawn แบบ Random ในกรอบ")]
    [SerializeField] private bool _spawnAtMouse = true;

    [Tooltip("ช่วง X สำหรับ Random Spawn (ใช้เมื่อ Spawn At Mouse = false)")]
    [SerializeField] private Vector2 _spawnRangeX = new Vector2(-5f, 5f);

    [Tooltip("ช่วง Y สำหรับ Random Spawn (ใช้เมื่อ Spawn At Mouse = false)")]
    [SerializeField] private Vector2 _spawnRangeY = new Vector2(-3f, 3f);

    [Header("── Utility Keys ────────────────────")]
    [Tooltip("กดปุ่มนี้เพื่อ Refresh")]
    [SerializeField] private KeyCode _refreshKey = KeyCode.Space;

    [Tooltip("กดปุ่มนี้เพื่อ Clear ไฟล์ทั้งหมด")]
    [SerializeField] private KeyCode _clearAllKey = KeyCode.X;

    [Tooltip("กดปุ่มนี้เพื่อ Reset Cortisol เป็น 0 (Debug)")]
    [SerializeField] private KeyCode _resetCortisolKey = KeyCode.C;

        [SerializeField] private KeyCode _gameoverKey  = KeyCode.C;
    private ScreenMateStats _screenMateStats;

    // -----------------------------------------------------------------------

    private void Awake()
    {
        _screenMateStats = FindAnyObjectByType<ScreenMateStats>();

        if (_screenMateStats == null)
            Debug.LogWarning("[DebugSpawner] ไม่พบ ScreenMateStats ในฉาก!");
    }

    private void Update()
    {
        foreach (SpawnEntry entry in _spawnEntries)
        {
            if (Input.GetKeyDown(entry.key))
            {
                Spawn(entry);
            }
        }

        if (Input.GetKeyDown(_refreshKey))
        {
            Debug.Log("[DebugSpawner] Refresh Command Invoked.");
            ActionCommands.OnRefreshCommand?.Invoke();
        }

        if (Input.GetKeyDown(_clearAllKey))
        {
            ClearAllFiles();
        }

        if (Input.GetKeyDown(_resetCortisolKey))
        {
            ResetCortisol();

        }
        if (Input.GetKeyDown(_gameoverKey))
        {
            ActionCommands.OnGameOver?.Invoke();
        }

    }

    // -----------------------------------------------------------------------

    /// <summary>
    /// Spawns a prefab at the mouse position or a random position.
    /// </summary>
    private void Spawn(SpawnEntry entry)
    {
        if (entry.prefab == null)
        {
            Debug.LogWarning($"[DebugSpawner] '{entry.label}' — Prefab ยังไม่ได้ลากใส่ Inspector!");
            return;
        }

        Vector3 spawnPos = _spawnAtMouse ? GetMouseWorldPosition() : GetRandomPosition();

        Instantiate(entry.prefab, spawnPos, Quaternion.identity);
    }

    /// <summary>
    /// Destroys all active BaseFile objects in the scene.
    /// </summary>
    private void ClearAllFiles()
    {
        BaseFile[] allFiles = FindObjectsByType<BaseFile>(FindObjectsInactive.Exclude);
        foreach (BaseFile file in allFiles)
        {
            Destroy(file.gameObject);
        }
        Debug.Log($"[DebugSpawner] Cleared {allFiles.Length} file(s).");
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        return pos;
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(_spawnRangeX.x, _spawnRangeX.y),
            Random.Range(_spawnRangeY.x, _spawnRangeY.y),
            0f
        );
    }

    private void ResetCortisol()
    {
        if (_screenMateStats == null)
        {
            Debug.LogWarning("[DebugSpawner] ResetCortisol failed — ScreenMateStats not found.");
            return;
        }

        _screenMateStats.DEBUG_ResetCortisol();
    }
}