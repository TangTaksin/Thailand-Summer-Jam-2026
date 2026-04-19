using UnityEngine;

public class GoodFile : ScreenElements, IEffectable
{
    public float heal = 15f;

    public void ApplyEffect(ScreenMateStats stats)
    {
        stats.UpdateCortisol(-heal);
        Destroy(gameObject);
    }
}
