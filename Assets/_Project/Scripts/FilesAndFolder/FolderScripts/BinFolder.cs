using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class BinFolder : BaseFolder
{
    [Header("Settings")]
    [SerializeField] private int _requiredFiles = 10;

    private readonly string[] _junkExtensions = { ".junk", ".trash", ".del", ".tmp" };

    [SerializeField] private List<GameObject> _storedFiles = new List<GameObject>();
    public int StoredFileCount => _storedFiles.Count;
    public bool CanEmpty => _storedFiles.Count >= _requiredFiles;

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
        foreach (GameObject fileObj in _storedFiles)
        {
            if (fileObj != null)
            {
                Destroy(fileObj);
            }
        }
        _storedFiles.Clear();
        Debug.Log("[BinFolder] Bin emptied.");

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
            _storedFiles.Add(junk.gameObject);
            junk.transform.SetParent(this.transform);
            junk.gameObject.SetActive(false);
        }
    }
}