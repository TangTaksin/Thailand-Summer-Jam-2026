using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    Camera mainCam;
    SpriteRenderer spriteRenderer;
    BoxCollider2D col2D;
    [SerializeField] SelectionBox selBox;

    InputAction action_point;
    InputAction action_click;
    InputAction action_rightClick;

    [SerializeField] List<ScreenElements> inSelection = new List<ScreenElements>();

    Vector3 mouseClickPos;
    bool isDragging;
    bool isGroupSelecting;

    void Awake()
    {
        mainCam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        col2D = GetComponent<BoxCollider2D>();

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



    #region Pointer & Click

    void PointerUpdate(InputAction.CallbackContext ctx)
    {
        var mouseScrPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        var mouseVelo = Input.mousePositionDelta * Time.deltaTime;
        
        if (isDragging)
        {
            if (inSelection[0])
                inSelection[0].Drag(mouseScrPos, mouseVelo);
        }

        if (isGroupSelecting)
        {
            UpdateSelectionBound(mouseClickPos, mouseScrPos);
        }
    }

    void Click(InputAction.CallbackContext ctx)
    {
        // clear selections

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var found = Physics2D.OverlapCircle(mousePos, .1f);

        mouseClickPos = mousePos;

        //ClearConditionCheck();

        if (found)
        {
            var scr_ele = found.GetComponent<ScreenElements>();

            // if it find screen element
            // add it to selection and start draging it
            if (scr_ele)
            {
                if (scr_ele is not SelectionBox)
                {
                    ClearSelection();
                }
                    AddSelection(scr_ele);

                    // store offset to each selected screen element
                    foreach (var ele in inSelection)
                    {
                        ele.UpdateOffset(mouseClickPos);
                    }

                isDragging = true;
                
            }
        }
        // else start box selection
        else
        {
            ClearSelection();

            print("Start Gruup Select");
            isGroupSelecting = true;
            EnableCollider(mouseClickPos);
        }
    }

    void ClickRelease(InputAction.CallbackContext ctx)
    {
        print("release");
        isDragging = false;

        if (isGroupSelecting)
        {
            selBox.AddtoBox(inSelection.ToArray());
            isGroupSelecting = false;
            DisableCollider();
            
        }
    }

    void RightClick(InputAction.CallbackContext ctx)
    {
        print("right click");
    }

    #endregion



    #region Selection Func.

    void AddSelection(ScreenElements scr_ele)
    {
        if (scr_ele is SelectionBox && isGroupSelecting)
            return;

        if (!inSelection.Contains(scr_ele))
        {
            inSelection.Add(scr_ele);
            scr_ele.StateOverride(ScreenElements.ScreenElementState.Freeze);
        
            if (isGroupSelecting)
                selBox.UpdateBoxBound(inSelection.ToArray());
        }
    }

    void RemoveSelection(ScreenElements scr_ele)
    {
        if (inSelection.Contains(scr_ele))
        {
            inSelection.Remove(scr_ele);
            scr_ele.StateOverride(ScreenElements.ScreenElementState.Normal);

            if (isGroupSelecting)
                selBox.UpdateBoxBound(inSelection.ToArray());
        }
    }

    void ClearSelection()
    {
        foreach(var ele in inSelection)
        {
            ele.StateOverride(ScreenElements.ScreenElementState.Normal);
        }

        selBox.HideBox();
        inSelection.Clear();
        
    }

    #endregion



    #region GroupSelectBound

    public void EnableCollider(Vector2 mousePos)
    {
        transform.position = mousePos;
    }

    public void DisableCollider()
    {
        UpdateSelectionBound(Vector2.zero,Vector2.zero);
    }

    public void UpdateSelectionBound(Vector2 startPoint, Vector2 endPoint)
    {
        var center = (startPoint + endPoint) / 2;
        transform.position = center;
        col2D.size = new Vector2(Mathf.Abs(endPoint.x - startPoint.x), Mathf.Abs(startPoint.y - endPoint.y));
        spriteRenderer.size = col2D.size;
    }

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent<ScreenElements>(out var scr_ele);

        if (scr_ele)
        {
            AddSelection(scr_ele);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        collision.TryGetComponent<ScreenElements>(out var scr_ele);

        if(scr_ele)
        {
            RemoveSelection(scr_ele);
        }
    }

    #endregion
}
