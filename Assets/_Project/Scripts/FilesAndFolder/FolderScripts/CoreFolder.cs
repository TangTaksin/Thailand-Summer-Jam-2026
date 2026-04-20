using UnityEngine;
using TMPro;
using System;

public class CoreFolder : BaseFolder
{
    [SerializeField] int _targetCount = 3;
    [SerializeField] TextMeshPro _textCount;

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
}