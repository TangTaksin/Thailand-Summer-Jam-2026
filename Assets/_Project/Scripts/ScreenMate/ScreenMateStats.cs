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




    private void Start()
    {
        if (_cortisolSlider) _cortisolSlider.maxValue = _maxCortisol;
    }

    private void Update()
    {
        UpdateCortisol(_passiveIncreaseRate * Time.deltaTime);
    }

    public void UpdateCortisol(float amount)
    {
        if (_isGameOver) return;

        CurrentCortisol = Mathf.Clamp(CurrentCortisol + amount, 0, _maxCortisol);

        if (_cortisolSlider) _cortisolSlider.value = CurrentCortisol;

        if (CurrentCortisol >= _maxCortisol)
        {
            _isGameOver = true;
            Debug.LogError("Game Over!");
            ActionCommands.OnGameOver?.Invoke();
        }
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

        if (_cortisolSlider != null)
            _cortisolSlider.value = 0f;

        Debug.Log("[ScreenMateStats] DEBUG: Cortisol reset to 0.");
    }
}