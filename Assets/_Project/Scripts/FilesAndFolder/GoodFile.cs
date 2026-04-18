using UnityEngine;

public class GoodFile : BaseFile, IEffectable
{
    [Header("Healing Effect")]
    public float healAmount = 15f;
    public void ApplyEffect(ScreenMateStats stats)
    {
        if (curloadSteps == 0)
        {
            stats.UpdateCortisol(-healAmount);
            Destroy(gameObject);
        }

    }
}
