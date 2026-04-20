using UnityEngine;

public class ActiveBadFile : BadFile
{
    protected override void Start()
    {
        curloadSteps = 0;
        InitializeHp();
        FindScreenMateTarget();
        LoadFile();
    }
}
