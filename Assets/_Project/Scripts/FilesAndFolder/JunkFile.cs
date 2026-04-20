using UnityEngine;
using System.IO;

public class JunkFile : BaseFile
{
    [SerializeField] private GameObject badFilePrefab;
    private readonly string[] junkExtensions = { ".junk", ".trash", ".del", ".tmp" };


    public override bool CanDelete(out string message)
    {
        message = "This file cannot be deleted. It is a junk file.";
        return false;
    }

    protected override void LoadFile()
    {
        if (CurLoadSteps == 0)
        {
            if (fileNameTextMeshPro != null) fileNameTextMeshPro.text = _loadedFileName;
            if (badFilePrefab != null)
            {
                GameObject virus = Instantiate(badFilePrefab, transform.position, transform.rotation);
                var badFileScript = virus.GetComponent<BadFile>();
                Destroy(gameObject);
            }
        }
        else
        {
            base.LoadFile();
        }
    }

    protected override void GenerateComplexBrokenName()
    {
        string hexCode = "0x" + Random.Range(0x0100, 0xFFFF).ToString("X");
        string ext = junkExtensions[Random.Range(0, junkExtensions.Length)];
        brokenFileName = hexCode + ext;

        if (!string.IsNullOrEmpty(_loadedFileName))
        {
            string cleanName = Path.GetFileNameWithoutExtension(_loadedFileName);
            _loadedFileName = cleanName + ext;
        }
    }
}