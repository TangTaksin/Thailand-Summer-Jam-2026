using UnityEngine;

public class GoodFile : BaseFile, IEffectable
{
    [Header("Healing Effect")]
    public float healAmount = 15f;
    public void ApplyEffect(ScreenMateStats stats)
    {
        if (!isLoaded)
        {
            Debug.Log("File is not loaded yet.");
            return;
        }

        stats.UpdateCortisol(-healAmount);
        Destroy(gameObject);
    }
}
