using UnityEngine;
using System.IO;

public class JunkFile : BaseFile
{
    [Header("Junk File Settings")]
    [Tooltip("นามสกุลเฉพาะสำหรับไฟล์ขยะ")]
    private readonly string[] junkExtensions = { ".junk", ".trash", ".del", ".tmp" };

    protected override void Start()
    {
        base.Start();
    }

    protected override void GenerateComplexBrokenName()
    {
        string hexCode = "0x" + Random.Range(0x0100, 0xFFFF).ToString("X");
        string ext = junkExtensions[Random.Range(0, junkExtensions.Length)];

        brokenFileName = hexCode + ext;

        if (!string.IsNullOrEmpty(loadedFileName))
        {
            string cleanName = Path.GetFileNameWithoutExtension(loadedFileName);
            loadedFileName = cleanName + ext;
        }
    }


}