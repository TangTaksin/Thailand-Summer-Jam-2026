using UnityEngine;
using UnityEngine.UI;

public class ScreenMateStats : MonoBehaviour
{
    public Slider[] _cortisolSliders;
    public float _maxCortisol = 100f;

    [Header("Passive Increase Settings")]
    public float _passiveIncreaseRate = 2f;

    public float CurrentCortisol { get; private set; } = 0f;
    public float MaxCortisol => _maxCortisol;

    private bool _isGameOver = false;

    public float _invincibleTimer = 0f;
    public bool _isInvincible => _invincibleTimer > 0;
    private bool _wasInvincible = false;

    [SerializeField] private SpriteRenderer spriteRenderer;



    private void Start()
    {
        if (_cortisolSliders != null)
        {
            foreach (Slider slider in _cortisolSliders)
            {
                if (slider != null)
                {
                    slider.maxValue = _maxCortisol;
                    slider.minValue = 0f;
                    slider.value = 0f;

                }

            }

        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_invincibleTimer > 0)
        {
            _invincibleTimer -= Time.deltaTime;
            _invincibleTimer = Mathf.Max(0f, _invincibleTimer);
        }

        UpdateCortisol(_passiveIncreaseRate * Time.deltaTime);

        if (spriteRenderer != null)
        {
            if (_isInvincible)
            {
                if (!_wasInvincible)
                {
                    Debug.Log("Invincible Mode! ON");
                    _wasInvincible = true;
                }
            }
            else
            {
                spriteRenderer.color = Color.white;

                if (_wasInvincible)
                {
                    Debug.Log("Invincible Mode! OFF");
                    _wasInvincible = false;
                }
            }
        }
    }

    public void UpdateCortisol(float amount)
    {
        if (_isGameOver) return;

        if (_isInvincible && amount > 0)
        {
            return;
        }

        CurrentCortisol = Mathf.Clamp(CurrentCortisol + amount, 0, _maxCortisol);

        if (_cortisolSliders != null)
        {
            foreach (Slider slider in _cortisolSliders)
            {
                if (slider != null)
                {
                    slider.value = CurrentCortisol;
                }
            }


        }

        if (CurrentCortisol >= _maxCortisol)
        {
            _isGameOver = true;
            Debug.LogError("Game Over!");
            AudioManager.Instance.PlaySFX("GameOver");
            ActionCommands.OnGameOver?.Invoke();
        }
    }

    public void ActivateInvincibility(float duration)
    {
        _invincibleTimer = Mathf.Max(_invincibleTimer, duration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IStatModifier effect = other.GetComponent<IStatModifier>();
        if (effect != null)
        {
            effect.ApplyModifier(this);
        }

        BaseFile file = other.GetComponent<BaseFile>();

        if (file != null)
        {
            if (file.CurLoadSteps == 0)
            {
                ActionCommands.OnFileEaten?.Invoke(file);

                if (!(file is BadFile))
                {
                    UpdateCortisol(-10f);
                    Debug.Log($"[Eat] กิน {file.gameObject.name} แล้ว! ลุ้น Core File...");
                }

                Destroy(file.gameObject);
            }
        }
    }

    public void DEBUG_ResetCortisol()
    {
        _isGameOver = false;
        CurrentCortisol = 0f;
        _invincibleTimer = 0f;
        _wasInvincible = false;

        if (_cortisolSliders != null)
        {
            foreach (Slider slider in _cortisolSliders)
            {
                if (slider != null) slider.value = 0f;
            }
        }

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        Debug.Log("[ScreenMateStats] DEBUG: Cortisol reset to 0.");
    }
}