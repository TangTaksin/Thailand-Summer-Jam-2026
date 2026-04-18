using UnityEngine;

public class BadFile : BaseFile, IEffectable
{
    [Header("Damage Effect")]
    public float damage = 20f;

    public void ApplyEffect(ScreenMateStats stats)
    {
        if (curloadSteps == 0)
        {
            stats.UpdateCortisol(damage);
            Destroy(gameObject);
        }
    }

}
