using UnityEngine;

public class GoodFile : BaseFile, IStatModifier
{
    [Header("Healing Effect")]
    public float healAmount = 15f;
    public void ApplyModifier(ScreenMateStats stats)
    {
        if (CurLoadSteps == 0)
        {
            stats.UpdateCortisol(-healAmount);
            Destroy(gameObject);
        }

    }
}
