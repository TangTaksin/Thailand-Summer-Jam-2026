using UnityEngine;

public class BaseBullet : MonoBehaviour, IStatModifier, IRefreshable
{
    [SerializeField] private float _bulletDamage = 10f;
    [SerializeField] private float _lifeTime = 4f;

    void OnEnable()
    {
        ActionCommands.OnRefreshCommand += Refresh;
    }

    void OnDisable()
    {
        ActionCommands.OnRefreshCommand -= Refresh;
    }

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }
    public void ApplyModifier(ScreenMateStats stats)
    {
        stats.UpdateCortisol(_bulletDamage);
        Destroy(gameObject);
    }

    public void Refresh()
    {
        Destroy(gameObject);
    }
}
