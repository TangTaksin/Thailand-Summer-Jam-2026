using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


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

        Cursor.lockState = CursorLockMode.Confined;

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
        action_click.canceled -= ClickRelease;
        action_rightClick.performed -= RightClick;
    }



    #region Pointer & Click

    void PointerUpdate(InputAction.CallbackContext ctx)
    {
        if (!mainCam)
            mainCam = Camera.main;

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
        if (IsPointerOverUI()) return;

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

        AudioManager.Instance.PlaySFX("Click");

    }

    void ClickRelease(InputAction.CallbackContext ctx)
    {
        print("release");

        if (isDragging)
        {
            isDragging = false;
            //reset state ของทุก element ที่ถูก freeze ตอน drag
            foreach (var ele in inSelection)
            {
                ele.StateOverride(ScreenElementState.Normal);
            }
        }

        // sent inSelection list to the box
        if (isGroupSelecting)
        {
            var selection_storage = inSelection.ToArray();
            isGroupSelecting = false;
            DisableCollider();


            selBox.AddtoBox(selection_storage);
        }
    }

    void RightClick(InputAction.CallbackContext ctx)
    {
        if (IsPointerOverUI()) return;

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
            }
        }
        // else clear
        else
        {
            ClearSelection();
        }
        AudioManager.Instance.PlaySFX("Click");
    }

    #endregion



    #region Selection Func.

    void AddSelection(ScreenElements scr_ele)
    {
        if (scr_ele is SelectionBox && isGroupSelecting)
            return;

        //ScreenMate ไม่ถูก group select
        if (isGroupSelecting && !scr_ele.IsGroupSelectable)
            return;

        if (!inSelection.Contains(scr_ele))
        {
            inSelection.Add(scr_ele);
            scr_ele.StateOverride(ScreenElementState.Freeze);

            if (isGroupSelecting)
                selBox.UpdateBoxBound(inSelection.ToArray());
        }
    }

    void RemoveSelection(ScreenElements scr_ele)
    {
        if (inSelection.Contains(scr_ele))
        {
            inSelection.Remove(scr_ele);
            scr_ele.StateOverride(ScreenElementState.Normal);

            if (isGroupSelecting)
                selBox.UpdateBoxBound(inSelection.ToArray());
        }
    }

    void ClearSelection()
    {
        foreach (var ele in inSelection)
        {
            ele.StateOverride(ScreenElementState.Normal);
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
        UpdateSelectionBound(Vector2.zero, Vector2.zero);
    }

    public void UpdateSelectionBound(Vector2 startPoint, Vector2 endPoint)
    {
        var center = (startPoint + endPoint) / 2;
        transform.position = center;
        col2D.size = new Vector2(Mathf.Abs(endPoint.x - startPoint.x), Mathf.Abs(startPoint.y - endPoint.y));
        spriteRenderer.size = col2D.size;
    }

    // 💡 ฟังก์ชันใหม่: ให้มันยิง Raycast เช็ค UI หน้าจอโดยตรง
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition; // ใช้ตำแหน่งเมาส์ปัจจุบัน
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0; // ถ้าเจอ UI มากกว่า 0 ชิ้น แปลว่าเมาส์ทับ UI อยู่!
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

        if (scr_ele)
        {
            RemoveSelection(scr_ele);
        }
    }

    #endregion
}