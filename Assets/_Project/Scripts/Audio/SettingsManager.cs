using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("UI Settings")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private float animationDuration = 0.5f;

    private Animator animator;
    private bool isPanelOpen = false;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (settingsPanel != null)
        {
            animator = settingsPanel.GetComponent<Animator>();
            
            if (animator != null)
            {
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            
            settingsPanel.SetActive(false);
        }
    }

    private void Start()
    {
        LoadVolume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleSettingsPanel();
        }
    }

    // ─── Audio Volume Controls ───

    public void SetMusicVolume() => SetVolume("Music", musicSlider, "musicVolume");
    public void SetSFXVolume() => SetVolume("SFX", sfxSlider, "sfxVolume");

    private void SetVolume(string exposedParam, Slider slider, string playerPrefKey)
    {
        if (audioMixer == null || slider == null) return;

        float value = slider.value;
        float dbValue = Mathf.Log10(Mathf.Max(0.0001f, value)) * 20f;

        audioMixer.SetFloat(exposedParam, dbValue);
        PlayerPrefs.SetFloat(playerPrefKey, value);
        PlayerPrefs.Save();
    }

    public void LoadVolume()
    {
        SetSliderValue(musicSlider, "musicVolume", "Music", 0.5f);
        SetSliderValue(sfxSlider, "sfxVolume", "SFX", 0.5f);
    }

    private void SetSliderValue(Slider slider, string key, string exposedParam, float defaultValue)
    {
        if (audioMixer == null || slider == null) return;

        float savedValue = PlayerPrefs.GetFloat(key, defaultValue);
        slider.value = savedValue;

        float dbValue = Mathf.Log10(Mathf.Max(0.0001f, savedValue)) * 20f;
        audioMixer.SetFloat(exposedParam, dbValue);
    }

    // ─── UI Panel Controls ───

    public void ToggleSettingsPanel()
    {
        if (isTransitioning) return; 

        if (isPanelOpen)
            CloseSettingsPanel();
        else
            OpenSettingsPanel();
    }

    public void OpenSettingsPanel()
    {
        if (settingsPanel == null) return;
        
        settingsPanel.SetActive(true);
        Time.timeScale = 0f;
        
        if (animator != null)
        {
            animator.enabled = true;
            animator.Play("Open_setting_UI_anim");
            StartCoroutine(WaitAnimationFinish());
        }
        isPanelOpen = true;
    }

    public void CloseSettingsPanel()
    {
        if (animator != null)
        {
            animator.Play("Close_Setting_ui_anim");
            StartCoroutine(DeactivatePanelAfterAnimation());
        }
        else
        {
            settingsPanel.SetActive(false);
            isPanelOpen = false;
            Time.timeScale = 1f;
        }
    }

    private IEnumerator WaitAnimationFinish()
    {
        isTransitioning = true;
        yield return new WaitForSecondsRealtime(animationDuration);
        isTransitioning = false;
    }

    private IEnumerator DeactivatePanelAfterAnimation()
    {
        isTransitioning = true;
        yield return new WaitForSecondsRealtime(animationDuration);
        
        settingsPanel.SetActive(false);
        isPanelOpen = false;
        isTransitioning = false;
        
        Time.timeScale = 1f; 
    }

    // ─── Button Actions ───

    public void Resume()
    {
        if (isTransitioning) return;
        CloseSettingsPanel();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}