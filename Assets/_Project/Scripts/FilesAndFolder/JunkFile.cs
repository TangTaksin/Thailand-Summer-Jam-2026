using UnityEngine;
using System.IO;

public class JunkFile : BaseFile
{
    [SerializeField] private GameObject _badFilePrefab;

    private static readonly string[] _junkExtensions = { ".junk", ".trash", ".del", ".tmp" };

    public override bool CanDelete(out string message)
    {
        message = "This file cannot be deleted. It is a junk file.";
        return false;
    }

    protected override void LoadFile()
    {
        if (CurLoadSteps > 0)
        {
            base.LoadFile();
            return;
        }

        SpawnVirus();
    }

    protected override void GenerateComplexBrokenName()
    {
        string hexCode = "0x" + Random.Range(0x0100, 0xFFFF).ToString("X");
        string ext = _junkExtensions[Random.Range(0, _junkExtensions.Length)];
        brokenFileName = hexCode + ext;

        if (!string.IsNullOrEmpty(_loadedFileName))
        {
            string cleanName = Path.GetFileNameWithoutExtension(_loadedFileName);
            _loadedFileName = cleanName + ext;
        }
    }


    private void SpawnVirus()
    {
        if (_badFilePrefab == null)
        {
            Debug.LogWarning($"[JunkFile] badFilePrefab is not assigned on {gameObject.name}!");
            return;
        }

        Instantiate(_badFilePrefab, transform.position, transform.rotation);
        Debug.Log($"[JunkFile] Virus spawned from {gameObject.name}.");
        Destroy(gameObject);
    }
}