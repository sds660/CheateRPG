using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
 
public class CritchGrid<TGridObject>
{
    private int _width;
    private int _height;
    private TGridObject[,] _gridArray;

    public event Action<int, int, string> OnGridChangedEvent;
    public CritchGrid (int width, int height, Func<CritchGrid<TGridObject>, int, int, TGridObject> createGridObj)
    {
        this._width = width;
        this._height = height;

        _gridArray = new TGridObject[width, height];

        ResetGrid(createGridObj);
    }
    public void ResetGrid(Func<CritchGrid<TGridObject>, int, int, TGridObject> createGridObj)
    {
        for (var i = 0; i < _gridArray.GetLength(0); i++)
        {
            for (var j = 0; j < _gridArray.GetLength(1); j++)
            {
                _gridArray[i, j] = createGridObj(this, i, j);
                OnGridChangedEvent?.Invoke(i, j, _gridArray[i, j].ToString());
            }
        }
    }
    public int GetWidth()
    {
        return _width;
    }
    public int GetHeight()
    {
        return _height;
    }
    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height) {
            _gridArray[x, y] = value;
            OnGridChangedEvent?.Invoke(x, y, value.ToString());
        }
    }
    public TGridObject GetGridObject(int x, int y) 
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height) {
            return _gridArray[x, y];
        } else {
            return default(TGridObject);
        }
    }
    public void TriggerGridObjectChanged(int x1, int y1, string val)
    {
        OnGridChangedEvent?.Invoke(x1, y1, val);
    }
    public TextMeshPro CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAlignmentOptions textAlignment) {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMeshPro));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        return textMesh;
    }
    public void CreateTextHere(string text, int fontSize, Vector3 position)
    {
        var newGO = new GameObject("test", typeof(TextMeshPro));
            
        newGO.transform.position = position;
        var myText = newGO.GetComponent<TextMeshPro>();
        myText.alignment = TextAlignmentOptions.Center;
        myText.fontSize = fontSize;
    }
}