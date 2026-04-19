using UnityEngine;

public class BadFile : BaseFile, IStatModifier, IMovable
{
    [Header("Damage Effect")]
    public float damage = 20f;

    [field: SerializeField] public float MoveSpeed { get; set; } = 2f; 

    private Transform targetScreenMate;

    protected override void Start()
    {
        base.Start();

        ScreenMateStats mate = FindAnyObjectByType<ScreenMateStats>();
        if (mate != null)
        {
            targetScreenMate = mate.transform;
        }
    }

    private void Update()
    {
        if (curloadSteps == 0 && targetScreenMate != null)
        {
            MoveToTarget(targetScreenMate);
        }
    }

    public void ApplyModifier(ScreenMateStats stats)
    {
        if (curloadSteps == 0)
        {
            stats.UpdateCortisol(damage);            
            Destroy(gameObject);
        }
    }
    public void MoveToTarget(Transform target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, MoveSpeed * Time.deltaTime);
    }
}