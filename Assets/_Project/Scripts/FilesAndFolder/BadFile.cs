using UnityEngine;

public class BadFile : BaseFile, IEffectable
{
    [Header("Damage Effect")]
    public float damage = 20f;

    public void ApplyEffect(ScreenMateStats stats)
    {

        if (!isLoaded)
        {
            Debug.Log("File is not loaded yet.");
            return;
        }

        stats.UpdateCortisol(damage);
        Destroy(gameObject);
    }

}
