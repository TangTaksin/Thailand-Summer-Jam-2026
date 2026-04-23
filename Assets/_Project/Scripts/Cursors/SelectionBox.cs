using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SelectionBox : ScreenElements
{
    SpriteRenderer boxRenderer;
    BoxCollider2D boxCollider2D;

    [SerializeField] ScreenElements[] insideTheBox = new ScreenElements[0];

    void Awake()
    {
        TryGetComponent<SpriteRenderer>(out boxRenderer);
        TryGetComponent<BoxCollider2D>(out boxCollider2D);

        HideBox();
    }

    void OnEnable()
    {
        ActionCommands.OnDeleteCommand += DeleteFilesInBox;
        ContextMenu.OnContextSelect += OnContextSelect;
    }

    void OnDisable()
    {
        ActionCommands.OnDeleteCommand -= DeleteFilesInBox;
        ContextMenu.OnContextSelect -= OnContextSelect;
    }

    void Update()
    {
        // จะทำงาน (ล็อกเป้า) ก็ต่อเมื่อมีไฟล์อยู่ข้างในกล่องเท่านั้น
        if (insideTheBox != null && insideTheBox.Length > 0)
        {
            // 💡 คำนวณขอบเขต Padding จากขนาดครึ่งหนึ่งของ Collider กล่อง
            // เพื่อให้ขอบกล่อง (ไม่ใช่แค่จุดกึ่งกลาง) ชนขอบจอพอดี
            Vector2 padding = new Vector2(boxCollider2D.size.x / 2f, boxCollider2D.size.y / 2f);

            // เรียกใช้ยันต์กันหลุดจอของเรา!
            transform.position = ScreenBoundary.ClampToScreen(transform.position, padding);
        }
    }

    public void HideBox()
    {
        FlushBox();
        boxRenderer.size = Vector2.zero;
        boxCollider2D.size = Vector2.zero;
    }

    public void UpdateBoxBound(ScreenElements[] inSelection)
    {
        var top = -Mathf.Infinity;
        var bottom = Mathf.Infinity;
        var right = -Mathf.Infinity;
        var left = Mathf.Infinity;

        foreach (var ele in inSelection)
        {
            var currentBound = ele.GetCollider().bounds;

            if (currentBound.max.y > top)
                top = currentBound.max.y;
            if (currentBound.min.y < bottom)
                bottom = currentBound.min.y;
            if (currentBound.max.x > right)
                right = currentBound.max.x;
            if (currentBound.min.x < left)
                left = currentBound.min.x;

            var size_x = Mathf.Abs(right - left);
            var size_y = Mathf.Abs(top - bottom);

            var coord_x = (right + left) / 2;
            var coord_y = (top + bottom) / 2;
            var center = new Vector2(coord_x, coord_y);

            transform.position = center;
            var box_size = new Vector2(size_x, size_y);

            boxRenderer.size = box_size;
            boxCollider2D.size = box_size;
        }
    }

    public void AddtoBox(ScreenElements[] inSelection)
    {
        insideTheBox = inSelection;

        foreach (var ele in inSelection)
        {
            ele.transform.SetParent(this.transform);
            ele.StateOverride(ScreenElementState.Freeze);
        }
    }

    public void FlushBox()
    {
        if (insideTheBox != null && insideTheBox.Length > 0)
        {
            foreach (var ele in insideTheBox)
            {
                // 💡 ใส่กันเหนียว เผื่อไฟล์โดนลบไปแล้วระหว่างอยู่ในกล่อง
                if (ele != null)
                {
                    ele.StateOverride(ScreenElementState.Normal);
                }
            }

            transform.DetachChildren();

            // 💡 สำคัญมาก! ต้องรีเซ็ต Array ให้ว่างเปล่า กล่องจะได้รู้ว่าตัวเองว่างแล้ว
            insideTheBox = new ScreenElements[0];
        }
    }

    private void DeleteFilesInBox()
    {
        // ถ้าไม่มีของในกล่อง ให้ข้ามไปเลย
        if (insideTheBox == null || insideTheBox.Length == 0) return;

        List<ScreenElements> remainingElements = new List<ScreenElements>();

        foreach (var ele in insideTheBox)
        {
            if (ele == null) continue;

            // เช็คว่าเป็น BaseFile ไหม จะได้เรียกฟังก์ชัน CanDelete ได้
            if (ele is BaseFile file)
            {
                string msg;
                if (file.CanDelete(out msg))
                {
                    // ลบได้ -> ตัดความสัมพันธ์และทำลายทิ้ง
                    file.transform.SetParent(null);
                    Destroy(file.gameObject);
                }
                else
                {
                    // ลบไม่ได้ (เช่น JunkFile) -> เก็บไว้เหมือนเดิม
                    remainingElements.Add(ele);
                }
            }
            else
            {
                // ถ้าไม่ใช่ BaseFile ลบทิ้งได้เลย
                ele.transform.SetParent(null);
                Destroy(ele.gameObject);
            }
        }

        // อัปเดตรายชื่อไฟล์ที่เหลืออยู่ในกล่อง
        insideTheBox = remainingElements.ToArray();

        // ถ้าลบหมดเกลี้ยง ก็ซ่อนกล่องทิ้งไป
        if (insideTheBox.Length == 0)
        {
            HideBox();
        }
    }

    void OnContextSelect()
    {
        HideBox();
    }
}
