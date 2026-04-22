using UnityEngine;

public class FirewallFile : BaseFile, IStatModifier
{
    [Header("Shield Settings")]
    [SerializeField] private float _shieldDuration = 3f;

    public void ApplyModifier(ScreenMateStats stats)
    {
        if (CurLoadSteps == 0)
        {
            stats.ActivateInvincibility(_shieldDuration);
            Destroy(gameObject);
        }
    }
}
