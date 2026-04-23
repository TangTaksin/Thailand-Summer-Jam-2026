using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class StartMenuManager : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
{
    Camera main_cam;

    [SerializeField] TextMeshProUGUI timeTMP;
    [SerializeField] TextMeshProUGUI dateTMP;
    [SerializeField] TextMeshProUGUI promptText;

    [SerializeField] Image backgroundImage;
    [SerializeField] GameObject scrollGroupObject;
     


    bool is_Draging;
    float drag_progress;
    [SerializeField] bool thresholdCrossed;

    Vector2 scroll_origin_position;
    Vector2 grabOffset;

    void Awake()
    {
        main_cam = Camera.main;

        scroll_origin_position = scrollGroupObject.transform.position;
    }

    void Update()
    {
        TimeUpdate();
    }

    void TimeUpdate()
    {
        
        var now = DateTime.Now;
        var timeText = string.Format("{0:t}", now);
        var dateText = string.Format("{0:d}", now);

        timeTMP.text = timeText;
        dateTMP.text = dateText;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        is_Draging = true;
        drag_progress = 0;
        grabOffset = scrollGroupObject.transform.position - Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        is_Draging = false;

        if (!thresholdCrossed)
        {
            drag_progress = 0;
            scrollGroupObject.transform.position = scroll_origin_position;
        }
        else
        {
            EnterGameplay();
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        var mouseMovement = Input.mousePositionDelta;

        if (is_Draging)
        {
            drag_progress += mouseMovement.y;
        }

        var veiwToScreen = main_cam.ViewportToScreenPoint(Vector3.up);
        thresholdCrossed = scrollGroupObject.transform.position.y > veiwToScreen.y;

        UpdateScreen(drag_progress);
    }

    void UpdateScreen(float progress)
    {
        var mousePos = Input.mousePosition;
        var OffsetedPos = (Vector2)mousePos + grabOffset;

        if (is_Draging)
        {
            scrollGroupObject.transform.position = new Vector2(scrollGroupObject.transform.position.x , OffsetedPos.y);
        }
    }

    # region LoadGameplaySequence

    void EnterGameplay()
    {
        var veiwToScreen = main_cam.ViewportToScreenPoint(Vector3.up * 1.5f);
        scrollGroupObject.transform.DOMoveY(veiwToScreen.y, 1f).SetEase(Ease.OutExpo);
        SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Additive);

        SceneManager.sceneLoaded += OnGameplayLoaded;
    }

    private void OnGameplayLoaded(Scene scene, LoadSceneMode loadmode)
    {
        SceneManager.sceneLoaded -= OnGameplayLoaded;
        
        if (scene.name == "Gameplay")
        {
            backgroundImage.DOFade(0,1f).SetEase(Ease.InExpo).onComplete = UnloadStartScene;
        }
    }

    void UnloadStartScene()
    {
        SceneManager.UnloadSceneAsync("StartMenu");
    } 

    #endregion
}
