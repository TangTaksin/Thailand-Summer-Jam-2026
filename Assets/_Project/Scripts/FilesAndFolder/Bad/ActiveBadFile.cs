using UnityEngine;

public class ActiveBadFile : BadFile
{
    protected override void Start()
    {
        base.Start();
        CurLoadSteps = 0;
        InitializeHp();
        FindScreenMateTarget();
        LoadFile();
    }
}
