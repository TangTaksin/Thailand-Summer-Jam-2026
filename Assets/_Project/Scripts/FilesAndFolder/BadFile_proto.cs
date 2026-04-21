using UnityEngine;

public class BadFile_proto : ScreenElements, IStatModifier
{
    public float damage = 20f;
    public void ApplyModifier(ScreenMateStats stats)
    {
        stats.UpdateCortisol(damage);
        Destroy(gameObject);
    }
}
