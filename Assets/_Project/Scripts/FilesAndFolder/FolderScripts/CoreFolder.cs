using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class CoreFolder : BaseFolder
{
    [SerializeField] int _targetCount = 3;
    [SerializeField] TextMeshPro _textCount;

    [Header("Feedback Settings")]
    [SerializeField] private float _bounceDistanceX = 2.5f;
    [SerializeField] private float _bounceHeight = 1.5f;
    [SerializeField] private float _bounceDuration = 0.3f;


    public int TargetCount => _targetCount;
    protected override bool CanAcceptFile(BaseFile file) => file is CoreFile;

    void OnEnable()
    {
        ActionCommands.OnFormatCommand += ExecuteFormat;
    }

    void OnDisable()
    {
        ActionCommands.OnFormatCommand -= ExecuteFormat;
    }

    void Start()
    {
        if (_textCount != null)
        {
            _textCount.text = _targetCount.ToString();
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


    protected override void ProcessFile(BaseFile file)
    {
        CoreFile core = file as CoreFile;
        // เช็คว่าโหลดเสร็จแล้ว (CurLoadSteps == 0)
        if (core != null && core.CurLoadSteps == 0)
        {
            _targetCount--;
            if (_targetCount < 0) _targetCount = 0;

            UpdateUI();
            Debug.Log($"[CoreFolder] Received: {file.gameObject.name} | Remaining: {_targetCount}");
            Destroy(file.gameObject);
        }

        if (_targetCount == 0)
        {
            Debug.Log("🎉 Ready to Format! Core files collected.");
        }
    }

    private void UpdateUI()
    {
        if (_textCount != null) _textCount.text = _targetCount.ToString();
    }

    private void ExecuteFormat()
    {
        if (_targetCount == 0)
        {
            Debug.Log("System Formatted!");
        }
    }

    private IEnumerator BounceFiles(BaseFile file)
    {
        Vector3 startPos = file.transform.position;

        float randomOffsetX = Random.Range(1, 12);
        float randomOffsetY = Random.Range(-4, 1);

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