using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InfluenceMapObject
{
    private CritchGrid<InfluenceMapObject> _grid;
    private int _x;
    private int _y;
    private int _value;
    private int _dist;

    public InfluenceMapObject(CritchGrid<InfluenceMapObject> grid, int x, int y, int dist)
    {
        this._grid = grid;
        this._x = x;
        this._y = y;
        this._dist = dist;

    }
    public int GetValue()
    {
        return _value;
    }
    public int GetDist()
    {
        return _dist;
    }
    public void SetDist(int dist)
    {
        _dist = dist;
        _grid.TriggerGridObjectChanged(_x, _y, _dist.ToString());
    }
    public void AddValue(int val)
    {
        _value += val;
        //_grid.TriggerGridObjectChanged(_x, _y, _value.ToString());
    }
    public int GetPosX()
    {
        return _x;
    }
    public int GetPosY()
    {
        return _y;
    }
    public override string ToString()
    {
        return _value.ToString();
    }
}