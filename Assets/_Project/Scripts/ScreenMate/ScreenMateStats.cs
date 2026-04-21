using System;
using UnityEngine;
using UnityEngine.UI;

public class ScreenMateStats : MonoBehaviour
{
    public Slider _cortisolSlider;
    public float _maxCortisol = 100f;

    [Header("Passive Increase Settings")]
    public float _passiveIncreaseRate = 2f;

    public float CurrentCortisol { get; private set; } = 0f;
    public float MaxCortisol => _maxCortisol;

    private bool _isGameOver = false;

    public float _invincibleTimer = 0f;
    public bool _isInvincible => _invincibleTimer > 0;

    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        if (_cortisolSlider) _cortisolSlider.maxValue = _maxCortisol;

        spriteRenderer = GetComponent<SpriteRenderer>();


    }

    private void Update()
    {
        if (_invincibleTimer > 0)
        {
            _invincibleTimer -= Time.deltaTime;
        }

        UpdateCortisol(_passiveIncreaseRate * Time.deltaTime);

        if (spriteRenderer != null)
        {
            if (_isInvincible)
            {
                spriteRenderer.color = Color.gold;
            }
            else
            {
                spriteRenderer.color = Color.white;
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

        if (_cortisolSlider) _cortisolSlider.value = CurrentCortisol;

        if (CurrentCortisol >= _maxCortisol)
        {
            _isGameOver = true;
            Debug.LogError("Game Over!");
            ActionCommands.OnGameOver?.Invoke();
        }
    }

    public void ActivateInvincibility(float duration)
    {
        _invincibleTimer = duration;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IStatModifier effect = other.GetComponent<IStatModifier>();
        if (effect != null)
        {
            effect.ApplyModifier(this);            
        }
    }

    public void DEBUG_ResetCortisol()
    {
        _isGameOver = false;
        CurrentCortisol = 0f;
        _invincibleTimer = 0f;

        if (_cortisolSlider != null)
            _cortisolSlider.value = 0f;

        Debug.Log("[ScreenMateStats] DEBUG: Cortisol reset to 0.");
    }
}