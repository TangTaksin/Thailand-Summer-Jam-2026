using UnityEngine;
using TMPro;

public class CoreFolder : BaseFolder
{
    [SerializeField] int _targetCount = 3;
    [SerializeField] TextMeshPro _textCount;

    void Start()
    {
        if (_textCount != null)
        {
            _textCount.text = _targetCount.ToString();
        }
    }

    protected override bool CanAcceptFile(BaseFile file) => file is CoreFile;

    protected override void ProcessFile(BaseFile file)
    {
        CoreFile core = file as CoreFile;
        if (core != null && core.CurLoadSteps == 0)
        {
            _targetCount--;
            
            if (_targetCount < 0) _targetCount = 0;

            if (_textCount != null)
            {
                _textCount.text = _targetCount.ToString();
            }

            Debug.Log($"[CoreFolder] Received: {file.gameObject.name} | Remaining: {_targetCount}");
            Destroy(file.gameObject);
        }

        if (_targetCount <= 0)
        {
            Debug.Log("Won! You have found all the core files!");
        }
    }
}