using UnityEngine;
using System.Linq;

public class BinFolder : BaseFolder
{
    private readonly string[] _junkExtensions = { ".junk", ".trash", ".del", ".tmp" };

    protected override bool CanAcceptFile(BaseFile file)
    {
        if (file == null || string.IsNullOrEmpty(file.LoadedFileName)) return false;

        return _junkExtensions.Any(ext => file.LoadedFileName.EndsWith(ext));
    }

    protected override void ProcessFile(BaseFile file)
    {
        if (file is JunkFile junk)
        {
            Debug.Log($"[BinFolder] Received junk file: {junk.gameObject.name}");
            Destroy(junk.gameObject);
        }
        else
        {
            Debug.LogWarning($"[BinFolder] Rejected non-junk file: {file.gameObject.name}");
        }
    }
}