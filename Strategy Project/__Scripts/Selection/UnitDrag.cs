using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDrag : MonoBehaviour
{
    Camera cam;

    //visual
    [SerializeField] RectTransform boxVisual;

    //logic
    Rect selectionBox;

    Vector2 startPos;
    Vector2 endPos;

    private void Start()
    {
        cam = Camera.main;
        startPos = Vector2.zero;
        endPos = Vector2.zero;
        DrawVisual();
    }
    void Update()
    {

        //when clicked
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            selectionBox = new Rect();

        }
        //when dragging
        if (Input.GetMouseButton(0))
        {
            endPos = Input.mousePosition;
            DrawVisual();
            DrawSelection();
        }
        //when release click
        if (Input.GetMouseButtonUp(0))
        {
            SelectUnits();
            startPos = Vector2.zero;
            endPos = Vector2.zero;
            DrawVisual();
        }
    }
    void DrawVisual()
    {
        Vector2 boxStart = startPos;
        Vector2 boxEnd = endPos;

        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        boxVisual.position = boxCenter;

        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

        boxVisual.sizeDelta = boxSize;
    }
    void DrawSelection()
    {
        // x calculations

        if (Input.mousePosition.x < startPos.x)
        {
            //dragging left
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = startPos.x;
        }
        else
        {
            selectionBox.xMin = startPos.x;
            selectionBox.xMax = Input.mousePosition.x;
            //dragging right
        }

        // y calculations

        if (Input.mousePosition.y < startPos.y)
        {
            //dragging down
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = startPos.y;
        }
        else
        {
            //dragging up
            selectionBox.yMin = startPos.y;
            selectionBox.yMax = Input.mousePosition.y;
        }
    }
    void SelectUnits()
    {
        /*
        foreach (var unit in UnitSelections.Instance.unitList)
        {
            //if unit is within the bounds of the selection rect
            if (selectionBox.Contains(cam.WorldToScreenPoint(unit.transform.position)))
            {
                UnitSelections.Instance.DragSelect(unit);
            }
        }
        */
    }
}
