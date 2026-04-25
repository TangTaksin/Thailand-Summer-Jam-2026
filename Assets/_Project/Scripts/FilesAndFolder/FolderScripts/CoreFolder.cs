using UnityEngine;
using TMPro;
using DG.Tweening;

public class CoreFolder : BaseFolder
{
    [SerializeField] int _targetCount = 3;
    [SerializeField] TextMeshPro _textCount;

    [Header("Feedback Settings")]
    [SerializeField] private float _bounceDistanceX = 2.5f;
    [SerializeField] private float _bounceHeight = 1.5f;
    [SerializeField] private float _bounceDuration = 0.3f;

    [Header("Scatter Ranges")]
    [SerializeField] private Vector2 _scatterX = new Vector2(1f, 5f);
    [SerializeField] private Vector2 _scatterY = new Vector2(-2f, 2f);

    private bool _isReadyAnimationPlaying = false;

    public int TargetCount => _targetCount;
    protected override bool CanAcceptFile(BaseFile file) => file is CoreFile;
    private Vector3 _initialScale;

    void OnEnable() { ActionCommands.OnFormatCommand += ExecuteFormat; }
    void OnDisable() { ActionCommands.OnFormatCommand -= ExecuteFormat; }

    void Awake()
    {
        _initialScale = transform.localScale;
    }

    void Start()
    {
        if (_textCount != null)
            _textCount.text = _targetCount.ToString();
    }

    public override void ReceiveFile(BaseFile droppedFile)
    {
        if (CanAcceptFile(droppedFile))
        {
            ProcessFile(droppedFile);
        }
        else
        {
            Debug.Log("[CoreFolder] File rejected: " + droppedFile.LoadedFileName);
            BounceFileWithTween(droppedFile);
            AudioManager.Instance.PlaySFX("FolderReject");
        }
    }

    protected override void ProcessFile(BaseFile file)
    {
        CoreFile core = file as CoreFile;
        if (core == null || core.CurLoadSteps != 0) return;

        _targetCount--;
        if (_targetCount < 0) _targetCount = 0;
        transform.DOKill();
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 10, 1);

        UpdateUI();
        Debug.Log($"[CoreFolder] Received: {file.gameObject.name} | Remaining: {_targetCount}");
        Destroy(file.gameObject);
        AudioManager.Instance.PlaySFX("DropFile");



        if (_targetCount == 0 && !_isReadyAnimationPlaying)
        {
            _isReadyAnimationPlaying = true;
            Debug.Log("Ready to Format! Core files collected.");

            transform.DOKill();
            transform.DOScale(_initialScale * 1.1f, 0.3f).SetEase(Ease.OutBack);
            transform.DOShakeRotation(0.5f, new Vector3(0, 0, 5f), 15, 90, false)
                     .SetLoops(-1, LoopType.Restart);
        }
    }

    private void UpdateUI()
    {
        if (_textCount == null) return;
        _textCount.text = _targetCount.ToString();
        _textCount.transform.DOKill();
        _textCount.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 8, 1);
    }

    private void ExecuteFormat()
    {
        if (_targetCount != 0) return;

        Debug.Log("System Formatted!");
        transform.DOKill();
        _isReadyAnimationPlaying = false;
        transform.DOPunchPosition(Vector3.down * 0.5f, 0.4f, 10, 1);
        transform.DOScale(_initialScale, 0.3f).SetEase(Ease.OutBack);
        transform.DORotate(Vector3.zero, 0.3f);
    }

    private void BounceFileWithTween(BaseFile file)
    {
        if (file == null) return;

        file.transform.DOKill();

        float randomOffsetX = Random.Range(_scatterX.x, _scatterX.y);
        float randomOffsetY = Random.Range(_scatterY.x, _scatterY.y);
        Vector3 endPos = transform.position + new Vector3(_bounceDistanceX + randomOffsetX, randomOffsetY, 0f);

        if (file is ProjectileFile projectile)
        {
            projectile.BounceTo(endPos, _bounceHeight, _bounceDuration);
        }
        else
        {
            file.transform.DOJump(endPos, _bounceHeight, 1, _bounceDuration).SetEase(Ease.OutQuad);
        }
    }
}