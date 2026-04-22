using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionBox : ScreenElements
{
    SpriteRenderer boxRenderer;
    BoxCollider2D boxCollider2D;

    ScreenElements[] insideTheBox;

    void Awake()
    {
        TryGetComponent<SpriteRenderer>(out boxRenderer);
        TryGetComponent<BoxCollider2D>(out boxCollider2D);

        HideBox();
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

        foreach(var ele in inSelection)
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

            var coord_x = (right + left)/2;
            var coord_y = (top + bottom)/2;
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
        }
    }

    public void FlushBox()
    {
        transform.DetachChildren();
    }
}
