using UnityEngine;

public class GoodFile_proto : ScreenElements, IStatModifier
{
    public float heal = 15f;
    public void ApplyModifier(ScreenMateStats stats)
    {
        stats.UpdateCortisol(-heal);
        Destroy(gameObject);
    }
}
