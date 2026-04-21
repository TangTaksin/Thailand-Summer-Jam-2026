using UnityEngine;

public class BadFile_proto : ScreenElements, IEffectable
{
    public float damage = 20f;

    public void ApplyEffect(ScreenMateStats stats)
    {
        stats.UpdateCortisol(damage);
        Destroy(gameObject);
    }
}
