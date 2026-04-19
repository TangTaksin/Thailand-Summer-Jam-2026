using UnityEngine;
using System.Linq;

public class BinFolder : BaseFolder
{
    private readonly string[] _junkExtensions = { ".junk", ".trash", ".del", ".tmp" };

    protected override bool CanAcceptFile(BaseFile file)
    {
        if (file == null || string.IsNullOrEmpty(file.loadedFileName)) return false;

        return _junkExtensions.Any(ext => file.loadedFileName.EndsWith(ext));
    }

    protected override void ProcessFile(BaseFile file)
    {
        JunkFile junk = file as JunkFile;
        if (junk != null && junk.curloadSteps == 0)
        {
            Debug.Log($"[BinFolder] Received: {file.gameObject.name}");
            Destroy(file.gameObject);
        }
    }
}