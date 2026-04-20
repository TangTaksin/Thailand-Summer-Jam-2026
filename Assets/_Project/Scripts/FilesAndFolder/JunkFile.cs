using UnityEngine;
using System.Collections;
using System.IO;

public class JunkFile : BaseFile
{
    [SerializeField] private GameObject badFilePrefab;
    private readonly string[] junkExtensions = { ".junk", ".trash", ".del", ".tmp" };

    protected override void LoadFile()
    {
        if (curloadSteps == 0)
        {
            if (fileNameTextMeshPro != null) fileNameTextMeshPro.text = loadedFileName;
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

        if (!string.IsNullOrEmpty(loadedFileName))
        {
            string cleanName = Path.GetFileNameWithoutExtension(loadedFileName);
            loadedFileName = cleanName + ext;
        }
    }
}