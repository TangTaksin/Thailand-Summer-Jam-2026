using UnityEngine;
using UnityEngine.UI;

public class ScreenMateStats : MonoBehaviour
{
    public Slider cortisolSlider;
    public float maxCortisol = 100f;

    [Header("Passive Increase Settings")]
    public float passiveIncreaseRate = 2f;

    private float currentCortisol = 0f;

    private void Start()
    {
        if (cortisolSlider) cortisolSlider.maxValue = maxCortisol;
    }

    private void Update()
    {
        UpdateCortisol(passiveIncreaseRate * Time.deltaTime);
    }

    public void UpdateCortisol(float amount)
    {
        currentCortisol = Mathf.Clamp(currentCortisol + amount, 0, maxCortisol);

        if (cortisolSlider) cortisolSlider.value = currentCortisol;

        if (currentCortisol >= maxCortisol)
        {
            Debug.LogError("Game Over!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
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
                return;
            }
        }

        IStatModifier effect = other.GetComponent<IStatModifier>();
        if (effect != null)
        {
            effect.ApplyModifier(this);
        }
    }
}