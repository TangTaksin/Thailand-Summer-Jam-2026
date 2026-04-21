using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening; // 💡 อย่าลืมเพิ่มตัวนี้!

public class BinFolder : BaseFolder
{
    [Header("Settings")]
    [SerializeField] private int _requiredFiles = 10;

    [Header("Feedback Settings")]
    [SerializeField] private float _bounceDistanceX = 2.5f;
    [SerializeField] private float _bounceHeight = 1.5f;
    [SerializeField] private float _bounceDuration = 0.5f;

    [Header("Scatter Ranges")]
    [SerializeField] private Vector2 _scatterX = new Vector2(1, 5);
    [SerializeField] private Vector2 _scatterY = new Vector2(-2, 2);

    private readonly string[] _junkExtensions = { ".junk", ".trash", ".del", ".tmp" };

    [SerializeField] private readonly List<GameObject> _storedFiles = new List<GameObject>();
    public int StoredFileCount => _storedFiles.Count;
    public bool CanEmpty => _storedFiles.Count >= _requiredFiles;
    private bool _isFullAnimationPlaying = false;

    void OnEnable() { ActionCommands.OnEmptyBinCommand += EmptyBin; }
    void OnDisable() { ActionCommands.OnEmptyBinCommand -= EmptyBin; }

    private void EmptyBin()
    {
        transform.DOKill();
        transform.DOPunchPosition(Vector3.down * 0.5f, 0.4f, 10, 1);

        foreach (GameObject fileObj in _storedFiles)
        {
            if (fileObj != null) Destroy(fileObj);
        }
        _storedFiles.Clear();

        transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutBack);
        transform.DORotate(Vector3.zero, 0.3f);

        BaseFile[] allFiles = FindObjectsByType<BaseFile>(FindObjectsInactive.Exclude);
        foreach (BaseFile file in allFiles) Destroy(file.gameObject);
    }

    public override void ReceiveFile(BaseFile droppedFile)
    {
        if (CanAcceptFile(droppedFile))
        {
            ProcessFile(droppedFile);
        }
        else
        {
            BounceFileWithTween(droppedFile);
        }
    }

    protected override bool CanAcceptFile(BaseFile file)
    {
        if (file == null || string.IsNullOrEmpty(file.LoadedFileName)) return false;

        foreach (string ext in _junkExtensions)
        {
            if (file.LoadedFileName.EndsWith(ext, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    protected override void ProcessFile(BaseFile file)
    {
        if (file is JunkFile junk)
        {
            _storedFiles.Add(junk.gameObject);
            junk.transform.SetParent(this.transform);
            junk.gameObject.SetActive(false);
            transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 10, 1);
        }

        if (_storedFiles.Count >= _requiredFiles && !_isFullAnimationPlaying)
        {
            _isFullAnimationPlaying = true;
            Debug.Log("Bin is full!");
            transform.DOKill();
            transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack);
            transform.DOShakeRotation(0.5f, new Vector3(0, 0, 5f), 15, 90, false)
                     .SetLoops(-1, LoopType.Restart);
        }

    }

    private void BounceFileWithTween(BaseFile file)
    {
        if (file == null) return;

        file.transform.DOKill();

        float randomOffsetX = Random.Range(_scatterX.x, _scatterX.y);
        float randomOffsetY = Random.Range(_scatterY.x, _scatterY.y);
        Vector3 endPos = transform.position + new Vector3(_bounceDistanceX + randomOffsetX, randomOffsetY, 0f);

        file.transform.DOJump(endPos, _bounceHeight, 1, _bounceDuration)
            .SetEase(Ease.OutQuad);

    }
}