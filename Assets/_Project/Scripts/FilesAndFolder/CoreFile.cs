using UnityEngine;
using System.IO;

public class CoreFile : BaseFile
{
    [Header("Core Settings")]
    private readonly string[] coreExtensions = { ".core", ".sys", ".key" };

    protected override void Start()
    {
        base.Start();
    }

    protected override void GenerateComplexBrokenName()
    {
        string hexCode = "0x" + Random.Range(0x1000, 0xFFFF).ToString("X");
        string ext = coreExtensions[Random.Range(0, coreExtensions.Length)];
        
        brokenFileName = hexCode + ext;

        if (!string.IsNullOrEmpty(loadedFileName))
        {
            string cleanName = Path.GetFileNameWithoutExtension(loadedFileName);
            loadedFileName = cleanName + ext;
        }
    }
}