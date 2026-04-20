using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class BinFolder : BaseFolder
{
    private readonly string[] _junkExtensions = { ".junk", ".trash", ".del", ".tmp" };


    [SerializeField] private List<GameObject> _storedFiles = new List<GameObject>();
    public int StoredFileCount => _storedFiles.Count;

    void OnEnable()
    {
        ActionCommands.OnEmptyBinCommand += EmptyBin;

    }

    private void OnDisable()
    {
        ActionCommands.OnEmptyBinCommand -= EmptyBin;

    }

    private void EmptyBin()
    {
        BaseFile[] allFiles = FindObjectsByType<BaseFile>(FindObjectsInactive.Exclude);
        foreach (BaseFile file in allFiles)
        {
            Destroy(file.gameObject);
        }
    }

    protected override bool CanAcceptFile(BaseFile file)
    {
        if (file == null || string.IsNullOrEmpty(file.LoadedFileName)) return false;

        return _junkExtensions.Any(ext => file.LoadedFileName.EndsWith(ext));
    }

    protected override void ProcessFile(BaseFile file)
    {
        if (file is JunkFile junk)
        {
            Debug.Log($"[BinFolder] Received junk file: {junk.gameObject.name}");
            _storedFiles.Add(junk.gameObject);
            Destroy(junk.gameObject);
        }
        else
        {
            Debug.LogWarning($"[BinFolder] Rejected non-junk file: {file.gameObject.name}");
        }
    }
}