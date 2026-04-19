using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    Camera mainCam;

    InputAction action_point;
    InputAction action_click;
    InputAction action_rightClick;

    [SerializeField] List<ScreenElements> inSelection = new List<ScreenElements>();

    Vector3 mouseClickPos;
    bool isDragging;

    void Awake()
    {
        mainCam = Camera.main;

        action_point = InputSystem.actions.FindAction("Point");
        action_click = InputSystem.actions.FindAction("Click");
        action_rightClick = InputSystem.actions.FindAction("RightClick");
    }

    void OnEnable()
    {
        action_point.performed += PointerUpdate;
        action_click.performed += Click;
        action_click.canceled += ClickRelease;
        action_rightClick.performed += RightClick;
    }

    void OnDisable()
    {
        action_point.performed -= PointerUpdate;
        action_click.performed -= Click;
        action_click.canceled += ClickRelease;
        action_rightClick.performed -= RightClick;
    }

    void PointerUpdate(InputAction.CallbackContext ctx)
    {
        var mouseScrPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        var mouseVelo = Input.mousePositionDelta * Time.deltaTime;
        
        if (isDragging)
        {
            inSelection[0]?.Drag(mouseScrPos, mouseVelo);
        }
    }

    void Click(InputAction.CallbackContext ctx)
    {
        // clear selections
        ClearSelection();

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var found = Physics2D.OverlapCircle(mousePos, .1f);

        if (found)
        {
            var scr_ele = found.GetComponent<ScreenElements>();

            print(scr_ele);

            // if it find screen element
            // add it to selection and start draging it
            if (scr_ele)
            {
                AddSelection(scr_ele);

                // store offset to each selected screen element
                mouseClickPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                scr_ele.UpdateOffset(mouseClickPos);
                isDragging = true;
                
            }
        }
        // else start box selection
    }

    void ClickRelease(InputAction.CallbackContext ctx)
    {
        print("release");
        isDragging = false;
    }

    void RightClick(InputAction.CallbackContext ctx)
    {
        print("right click");
    }


    void AddSelection(ScreenElements scr_ele)
    {
        scr_ele.StateOverride(ScreenElements.ScreenElementState.Freeze);
        inSelection.Add(scr_ele);
    }

    void ClearSelection()
    {
        foreach (var ele in inSelection)
        {
            ele.StateOverride(ScreenElements.ScreenElementState.Normal);
        }

        inSelection.Clear();
    }
}
