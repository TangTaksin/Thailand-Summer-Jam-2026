using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Hold Settings")]
    public float holdDuration = 5f;
    public Image progressImage;

    [Header("Events")]
    public UnityEvent onHoldComplete;

    private bool _isHolding = false;
    private bool _isAlreadyFired = false;
    private float _holdTimer = 0f;
    private Button _button;
    [Header("Audio Settings")]
    [SerializeField] private AudioSource _holdAudioSource;

    private void Start()
    {
        _button = GetComponent<Button>();
        ResetProgress();
    }

    private void Update()
    {
        if (_button == null || progressImage == null) return;

        if (_isHolding && _button.interactable && !_isAlreadyFired)
        {
            _holdTimer += Time.deltaTime;
            float progress = _holdTimer / holdDuration;

            if (progressImage != null)
            {
                progressImage.fillAmount = progress;
            }

            if (_holdAudioSource != null && _holdAudioSource.isPlaying)
            {
                _holdAudioSource.pitch = Mathf.Lerp(1.0f, 2.0f, progress);
            }

            if (_holdTimer >= holdDuration)
            {
                if (_holdAudioSource != null) _holdAudioSource.Stop();
                AudioManager.Instance.PlaySFX("Complete");
                onHoldComplete?.Invoke();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_button.interactable || _isAlreadyFired) return;
        _isHolding = true;

        if (_holdAudioSource != null)
        {
            _holdAudioSource.pitch = 1.0f;
            _holdAudioSource.Play();
        }
        _holdTimer = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isAlreadyFired) return;
        _isHolding = false;

        if (_holdAudioSource != null && !_isAlreadyFired)
        {
            _holdAudioSource.Stop();
        }
        ResetProgress();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isAlreadyFired) return;
        ResetProgress();
    }

    private void OnDisable()
    {
        ResetProgress();
    }

    private void ResetProgress()
    {
        _isHolding = false;
        _holdTimer = 0f;
        if (progressImage != null) progressImage.fillAmount = 0f;
    }

}