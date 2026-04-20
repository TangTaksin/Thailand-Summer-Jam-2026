using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    Camera mainCam;
    BoxCollider2D col2D;

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

    void PointerUpdate(InputAction.CallbackContext ctx)
    {
        var mouseScrPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        var mouseVelo = Input.mousePositionDelta * Time.deltaTime;
        
        if (isDragging)
        {
            foreach (var ele in inSelection)
            {
                if (ele)
                ele.Drag(mouseScrPos, mouseVelo);
            }
        }

        if (isGroupSelecting)
        {
            UpdateBox(mouseClickPos, mouseScrPos);
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

            print(scr_ele);

            // if it find screen element
            // add it to selection and start draging it
            if (scr_ele)
            {
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
            print("Start Gruup Select");
            isGroupSelecting = true;
            EnableCollider(mouseClickPos);
        }
    }

    void ClickRelease(InputAction.CallbackContext ctx)
    {
        print("release");
        isDragging = false;
        isGroupSelecting = false;
        DisableCollider();
    }

    void RightClick(InputAction.CallbackContext ctx)
    {
        print("right click");
    }


    void AddSelection(ScreenElements scr_ele)
    {
        if (!inSelection.Contains(scr_ele))
        {
            inSelection.Add(scr_ele);
            scr_ele.StateOverride(ScreenElements.ScreenElementState.Freeze);
        }
    }

    void RemoveSelection(ScreenElements scr_ele)
    {
        if (inSelection.Contains(scr_ele))
        {
            inSelection.Remove(scr_ele);
            scr_ele.StateOverride(ScreenElements.ScreenElementState.Normal);
        }
    }

    void ClearConditionCheck(ScreenElements scr_ele)
    {
        /// clear if :
        /// 1. selecting file that are not being selected
        /// 2. releasing click on blank space

        
    }

    #region GroupSelectBox

    public void EnableCollider(Vector2 mousePos)
    {
        transform.position = mousePos;
    }

    public void DisableCollider()
    {
        col2D.size = Vector2.zero;
    }

    public void UpdateBox(Vector2 startPoint, Vector2 endPoint)
    {
        var center = (startPoint + endPoint) / 2;
        transform.position = center;
        col2D.size = new Vector2(Mathf.Abs(endPoint.x - startPoint.x), Mathf.Abs(startPoint.y - endPoint.y));
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

        if(scr_ele && isGroupSelecting)
        {
            RemoveSelection(scr_ele);
        }
    }

    #endregion
}
