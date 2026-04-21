using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class BinFolder : BaseFolder
{
    [Header("Settings")]
    [SerializeField] private int _requiredFiles = 10;

    [Header("Feedback Settings")]
    [SerializeField] private float _bounceDistanceX = 2.5f;
    [SerializeField] private float _bounceHeight = 1.5f;
    [SerializeField] private float _bounceDuration = 0.3f;

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

    public override void ReceiveFile(BaseFile droppedFile)
    {
        if (CanAcceptFile(droppedFile))
        {
            ProcessFile(droppedFile);
        }
        else
        {
            Debug.Log("[BinFolder] File rejected: " + droppedFile.LoadedFileName);
            StartCoroutine(BounceFiles(droppedFile));
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

    private IEnumerator BounceFiles(BaseFile file)
    {
        Vector3 startPos = file.transform.position;

        float randomOffsetX = Random.Range(1, 12);
        float randomOffsetY = Random.Range(-2, 3);

        Vector3 endPos = startPos + new Vector3(_bounceDistanceX + randomOffsetX, -0.5f + randomOffsetY, 0f);

        float timeElapsed = 0f;

        while (timeElapsed < _bounceDuration)
        {
            timeElapsed += Time.deltaTime;

            float t = timeElapsed / _bounceDuration;

            float currentBounceHeight = _bounceHeight * Mathf.Sin(t * Mathf.PI);
            float heightModifier = Mathf.Sin(t * Mathf.PI) * currentBounceHeight;
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
            currentPos.y += heightModifier;

            if (file != null)
            {
                file.transform.position = currentPos;
            }

            yield return null;
        }

        if (file != null)
        {
            file.transform.position = endPos;
        }
    }
}