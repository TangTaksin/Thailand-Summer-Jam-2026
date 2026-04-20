using UnityEngine;

public class DebugSpawner : MonoBehaviour
{
    [Header("Prefabs to Test")]
    public GameObject junkFilePrefab;
    public GameObject coreFilePrefab;
    public GameObject goodFilePrefab;
    public GameObject badFilePrefab;

    [Header("Spawn Settings")]
    public bool spawnAtMouse = true;
    public Vector2 spawnRangeX = new Vector2(-5, 5);
    public Vector2 spawnRangeY = new Vector2(-3, 3);

    void Update()
    {
        // กด J เพื่อเสก Junk File
        if (Input.GetKeyDown(KeyCode.Q)) Spawn(junkFilePrefab, "Junk File");

        // กด C เพื่อเสก Core File
        if (Input.GetKeyDown(KeyCode.W)) Spawn(coreFilePrefab, "Core File");

        // กด G เพื่อเสก Good/Base File
        if (Input.GetKeyDown(KeyCode.E)) Spawn(goodFilePrefab, "Good File");

        // กด B เพื่อเสก Bad File (ไวรัส)
        if (Input.GetKeyDown(KeyCode.R)) Spawn(badFilePrefab, "Bad File");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActionCommands.OnRefreshCommand?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ClearAllFiles();
        }
    }

    private void ClearAllFiles()
    {
        BaseFile[] allFiles = FindObjectsByType<BaseFile>(FindObjectsInactive.Exclude);
        foreach (BaseFile file in allFiles)
        {
            Destroy(file.gameObject);
        }
    }

    private void Spawn(GameObject prefab, string label)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"[Debug] {label} Prefab ยังไม่ได้ลากใส่ใน Inspector นะ!");
            return;
        }

        Vector3 pos;
        if (spawnAtMouse)
        {
            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
        }
        else
        {
            pos = new Vector3(Random.Range(spawnRangeX.x, spawnRangeX.y),
                              Random.Range(spawnRangeY.x, spawnRangeY.y), 0);
        }

        Instantiate(prefab, pos, Quaternion.identity);
        Debug.Log($"[Debug] Spawned: {label} at {pos}");
    }
}
