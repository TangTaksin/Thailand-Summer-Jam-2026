using UnityEngine;
using DG.Tweening;

public class ProjectileFile : BadFile
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _projectileSpeed = 10f;
    [SerializeField] private float _fireRate = 2f;

    [Header("Movement Settings (Oscillation)")]
    [SerializeField] public float _frequency = 2f;
    [SerializeField] private float _amplitude = 1.5f;

    private float _fireTimer = 0f;
    private float _sineTimer;
    private Vector3 _startPos;

    //private bool _isDragging = false;
    private bool _isTweening = false;

    protected override void Start()
    {
        base.Start();
        _fireTimer = _fireRate;
        _startPos = transform.position;
    }

    private void Update()
    {
        bool canAction = 
        CurLoadSteps == 0 && 
        CurrentHp > 0 && 
        _targetScreenMate != null && 
        !_isTweening && 
        element_state == ScreenElementState.Normal;

        if (canAction)
        {
            HandleShooting();
            HandleMovement();
        }
    }

    private void HandleShooting()
    {
        _fireTimer -= Time.deltaTime;
        if (_fireTimer <= 0)
        {
            ShootAtTarget();
            _fireTimer = _fireRate;
        }
    }

    private void HandleMovement()
    {
        _sineTimer += Time.deltaTime;

        Vector2 dirToTarget = ((Vector2)_targetScreenMate.position - (Vector2)_startPos).normalized;
        Vector3 sideDir = new Vector2(-dirToTarget.y, dirToTarget.x);

        float sineOffset = Mathf.Sin(_sineTimer * _frequency * 2 * Mathf.PI) * _amplitude;
        transform.position = _startPos + (sideDir * sineOffset);
    }

    private void ShootAtTarget()
    {
        if (_bulletPrefab == null || _targetScreenMate == null) return;

        GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        Vector2 direction = (_targetScreenMate.position - transform.position).normalized;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction * _projectileSpeed;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void BounceTo(Vector3 targetPos, float height, float duration)
    {
        _isTweening = true;
        transform.DOKill();

        transform.DOJump(targetPos, height, 1, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                _startPos = transform.position;
                _sineTimer = 0f;
                _isTweening = false;
            });
    }

    protected override void OnNormalState()
    {
        base.OnNormalState();

        if (CurrentHp > 0)
        {
            _startPos = transform.position;
            _sineTimer = 0f;
        }
    }

    protected override void OnFreezeState()
    {
        base.OnFreezeState();

        transform.DOKill();
    }

    // protected override void OnMouseDown()
    // {
    //     //base.OnMouseDown();
    //     transform.DOKill();

    //     //_isDragging = true;

    // }

    // protected override void OnMouseUp()
    // {
    //     base.OnMouseUp();
    //     //_isDragging = false;

    //     if (CurrentHp > 0)
    //     {
    //         _startPos = transform.position;
    //         _sineTimer = 0f;
    //     }
    // }
}