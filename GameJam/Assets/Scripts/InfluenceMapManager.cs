using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class InfluenceMapManager : MonoBehaviour
{
    public static readonly Vector2Int[] CardinalDirections = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.right, 
        Vector2Int.down, 
        Vector2Int.left
    };
    
    public static readonly Vector2Int[] EightDirections = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.up + Vector2Int.right, 
        Vector2Int.right,
        Vector2Int.right + Vector2Int.down, 
        Vector2Int.down,
        Vector2Int.down + Vector2Int.left,
        Vector2Int.left,
        Vector2Int.up + Vector2Int.left
    };

    public static readonly Vector3 TilemapOffset = new Vector3(0, 0.25f, 0);
    
    private CritchGrid<InfluenceMapObject> _influenceGrid;
    private CritchGrid<InfluenceMapObject> _distanceGrid;
    
    [SerializeField] private GameObject tileMapObj;
    [SerializeField] private GameObject tmProContainer;
    [SerializeField] private int  startVal;
    [SerializeField] private int  influenceRange;
    [SerializeField] private int  distanceRange;
    [SerializeField] private bool showDebug;
    
    private Tilemap _groundTilemap;
    private Tilemap _collisionTilemap;
    
    private TextMeshPro[,] _debugTextArray;
    
    private int _groundTilemapOffsetX;
    private int _groundTilemapOffsetY;
    
    private int _collisionTilemapOffsetX;
    private int _collisionTilemapOffsetY;
    
    private MouseInput _mouseInput;
    
    private void Awake()
    {
        _mouseInput = new MouseInput();
        
        _groundTilemap = tileMapObj.transform.Find("Ground Map").GetComponent<Tilemap>();
        _collisionTilemap = tileMapObj.transform.Find("Collision Map").GetComponent<Tilemap>();
        
        _groundTilemap.CompressBounds();
        _collisionTilemap.CompressBounds();
        
        var bounds = _groundTilemap.size;
        var cellBounds = _groundTilemap.cellBounds;
        _groundTilemapOffsetX = cellBounds.xMin;
        _groundTilemapOffsetY = cellBounds.yMin;
        
        var collisionBounds = _collisionTilemap.size;
        var collisionCellBounds = _collisionTilemap.cellBounds;
        _collisionTilemapOffsetX = cellBounds.xMin;
        _collisionTilemapOffsetY = cellBounds.yMin;
        
        _influenceGrid = new CritchGrid<InfluenceMapObject>(Mathf.Abs(bounds.x), 
            Mathf.Abs(bounds.y),
            (g, x, y) => new InfluenceMapObject(g, x, y, 0));
        
        _distanceGrid = new CritchGrid<InfluenceMapObject>(Mathf.Abs(bounds.x), 
            Mathf.Abs(bounds.y),
            (g, x, y) => new InfluenceMapObject(g, x, y, 0));


        if (showDebug)
        {
            _debugTextArray = new TextMeshPro[_influenceGrid.GetWidth(), _influenceGrid.GetHeight()];

            for (var w = 0; w < _influenceGrid.GetWidth(); w++)
            {
                for (var h = 0; h < _influenceGrid.GetHeight(); h++)
                {
                    _debugTextArray[w, h] = TextAtMapPosition(new Vector3Int(w - + Mathf.Abs(_groundTilemapOffsetX), 
                            h - Mathf.Abs(_groundTilemapOffsetY), 0), 
                        _influenceGrid.GetGridObject(w,h).GetValue().ToString());
                }
            }
            //_influenceGrid.OnGridChangedEvent += UpdateTextAtPosition;
            _distanceGrid.OnGridChangedEvent += UpdateTextAtPosition;
        }

        //_mouseInput.Mouse.MouseClick.performed += _ => MouseClick();

    }
    private void OnEnable()
    {
        _mouseInput.Enable();
    }
    private void OnDisable()
    {
        _mouseInput.Disable();
    }
    private void Start()
    {
        // printing grid and tilemap positions
        // Debug.Log($"Grid height {_grid.GetHeight()}");
        // Debug.Log($"Grid width {_grid.GetWidth()}");
        //
        // Debug.Log($"Tilemap x {Mathf.Abs(bounds.x)}");
        // Debug.Log($"Tilemap y {Mathf.Abs(bounds.y)}");
    }
    public InfluenceMapObject getDistPos(int x, int y)
    {
        return _distanceGrid.GetGridObject(x, y);
    }
    public InfluenceMapObject getDistPos(Vector3 worldPos)
    {
        var gridPos = getGridPositionFromGroundMap(worldPos);
        return _distanceGrid.GetGridObject(gridPos.x, gridPos.y);
    }
    public InfluenceMapObject getInfluencePos(int x, int y)
    {
        return _influenceGrid.GetGridObject(x, y);
    }
    private void MouseClick()
    {
        var mousePos = _mouseInput.Controls.MousePosition.ReadValue<Vector2>();
        if (Camera.main is { }) mousePos = Camera.main.ScreenToWorldPoint(mousePos);
     
        Vector3Int tMapPos = _groundTilemap.WorldToCell(mousePos);
        Vector3Int collisionPos = _collisionTilemap.WorldToCell(mousePos);


        Debug.Log("World position from camera: " + mousePos.x + ", " + mousePos.y);
        Debug.Log("Grid position from function: " + getGridPositionFromGroundMap(mousePos).x + ", " + getGridPositionFromGroundMap(mousePos).y);
        Debug.Log("World position from function: " + getWorldPositionFromGrid(getGridPositionFromGroundMap(mousePos)).x + ", " + 
                                    getWorldPositionFromGrid(getGridPositionFromGroundMap(mousePos)).y);
        
        // if (_groundTilemap.HasTile(tMapPos))
        // {
        //     _influenceGrid.ResetGrid((g, x, y) => new InfluenceMapObject(g, x, y, 0));
        //     var imObj = _influenceGrid.GetGridObject(tMapPos.x + Mathf.Abs(_groundTilemapOffsetX), tMapPos.y + Mathf.Abs(_groundTilemapOffsetY));
        //     // if (imObj != null) Debug.Log($"Position {imObj.GetPosX()}, {imObj.GetPosY()} in grid!");
        //     // Debug.Log($"Position {tMapPos.x}, {tMapPos.y} in tileMap!");
        //     
        //     if (imObj != null)
        //     {
        //         imObj.SetDist(0);
        //         imObj.AddValue(startVal);
        //         Propagate(imObj.GetPosX(), imObj.GetPosY(), collisionPos, influenceRange, startVal, _influenceGrid);
        //     }
        // }
    }
    public Vector3Int getGridPositionFromGroundMap(Vector3 worldPos)
    {
        var tMapPos = _groundTilemap.WorldToCell(worldPos);
        return new Vector3Int(tMapPos.x + Mathf.Abs(_groundTilemapOffsetX), tMapPos.y + Mathf.Abs(_groundTilemapOffsetY), 0);
    }
    
    public Vector3 getWorldPositionFromGrid(Vector3Int gridPos)
    {
        var tilePos = new Vector3Int(gridPos.x - Mathf.Abs(_groundTilemapOffsetX),
            gridPos.y - Mathf.Abs(_groundTilemapOffsetY), 0);
        
        return _groundTilemap.CellToWorld(tilePos) + TilemapOffset;
    }
    public void CalculateDistance(Vector3 worldPos)
    {
        var tMapPos = _groundTilemap.WorldToCell(worldPos);
        var collisionPos = _collisionTilemap.WorldToCell(worldPos);
        
        var gridPos = getGridPositionFromGroundMap(worldPos);
        var imObj = _distanceGrid.GetGridObject(gridPos.x, gridPos.y);
            
        if (imObj != null)
        {
            _distanceGrid.ResetGrid((g, x, y) => new InfluenceMapObject(g, x, y, 0));
            // imObj.SetDist(0);
            imObj.AddValue(0);
            Propagate(imObj.GetPosX(), imObj.GetPosY(), distanceRange, 0, _distanceGrid);
        }
    }
    public void PropagateInfluence(Vector3 worldPos)
    {
        Vector3Int tMapPos = _groundTilemap.WorldToCell(worldPos);
        Vector3Int collisionPos = _collisionTilemap.WorldToCell(worldPos);
        
        var imObj = _influenceGrid.GetGridObject(tMapPos.x + Mathf.Abs(_groundTilemapOffsetX), tMapPos.y + Mathf.Abs(_groundTilemapOffsetY));
            
        if (imObj != null)
        {
            _influenceGrid.ResetGrid((g, x, y) => new InfluenceMapObject(g, x, y, 0));
            imObj.AddValue(startVal);
            Propagate(imObj.GetPosX(), imObj.GetPosY(), influenceRange, startVal, _influenceGrid);
        }
    }
    private void Propagate(int x, int y, int range, int startingVal, CritchGrid<InfluenceMapObject> map)
    {
        var openList = new Queue<Vector2Int>();
        openList.Enqueue(new Vector2Int(x, y));
        
        var closedList = new List<Vector2Int> {new Vector2Int(x, y)};

        var directions = CardinalDirections;

        if (range < 0) range = int.MaxValue;
        
        while (openList.Count != 0)
        {
            var curObj = openList.Dequeue();
            var curVal = map.GetGridObject(curObj.x, curObj.y).GetValue() / 2;
            
            
            
            //if (curVal <= 0 || map.GetGridObject(curObj.x, curObj.y).GetDist() >= range)
            if (map.GetGridObject(curObj.x, curObj.y).GetDist() >= range) 
            {
                continue;
            }
            
            foreach (var dir in directions)
            {
                Vector2Int newPos = new Vector2Int(curObj.x + dir.x, curObj.y + dir.y);
                
                if ((newPos.x >= 0) &&
                    (newPos.x <  map.GetWidth()) &&
                    (newPos.y >= 0) &&
                    (newPos.y <  map.GetHeight()))
                {
                    if (!closedList.Contains(newPos))
                    {
                        var gridObj = map.GetGridObject(newPos.x, newPos.y);
                        var worldPos =
                            getWorldPositionFromGrid(new Vector3Int(gridObj.GetPosX(), gridObj.GetPosY(), 0));

                        var collisionTile = _collisionTilemap.WorldToCell(worldPos);
                        var groundTile = _groundTilemap.WorldToCell(worldPos);
                        if (_collisionTilemap.HasTile(collisionTile) || !_groundTilemap.HasTile(groundTile)) continue;
                        
                        closedList.Add(new Vector2Int(newPos.x, newPos.y));
                        openList.Enqueue(newPos);
                        gridObj.SetDist(map.GetGridObject(curObj.x, curObj.y).GetDist() + 1);
                        gridObj.AddValue(startVal - gridObj.GetDist());
                    }
                }
            }
        }
    }

    private TextMeshPro TextAtMapPosition(Vector3Int tileMapPosition, string text)
    {
        GameObject newGO = new GameObject("test");
        Transform newGOtransform = newGO.transform;
        newGOtransform.position = _groundTilemap.GetCellCenterWorld(tileMapPosition) + TilemapOffset;
        newGOtransform.SetParent(tmProContainer.transform);
        TextMeshPro myText = newGO.AddComponent<TextMeshPro>();
        myText.alignment = TextAlignmentOptions.Center;
        myText.fontSize = 4;
        myText.text = text;

        return myText;
    }
    public void UpdateTextAtPosition(int x, int y, string text)
    {
        _debugTextArray[x, y].text = text;
    }
}