using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class MapFollow : MonoBehaviour
{
    [SerializeField] private GameObject influenceMapManagerObject;
    [SerializeField] private GameObject tileMapObj;
    [SerializeField] private int recalcTime = 30;

    private Tilemap _groundTilemap;
    private Tilemap _collisionTilemap;
    
    private int _frameCounter = 0;
    
    private IsoController _controller;
    
    private Vector3 _destination;
    
    private Vector3Int _prevTmapPos;
    private InfluenceMapManager _influenceManager;

    private void Awake()
    {
        if (influenceMapManagerObject == null)
        {
            influenceMapManagerObject = GameObject.Find("Influence Map Manager");
        }
        if (tileMapObj == null)
        {
            tileMapObj = GameObject.Find("Tilemap");
        }

        _influenceManager = influenceMapManagerObject.GetComponent<InfluenceMapManager>();
        TryGetComponent<IsoController>(out _controller);
        _groundTilemap = tileMapObj.transform.Find("Ground Map").GetComponent<Tilemap>();
        _collisionTilemap = tileMapObj.transform.Find("Collision Map").GetComponent<Tilemap>();
    }
    private void Start()
    {
        _destination = transform.position;
    }
    private Vector3 GetRandomWorldPosNearby()
    {
        var random = new Random();
        Vector3Int tilePos;
        var gridPos = _influenceManager.getGridPositionFromGroundMap(transform.position);
        var gridObj = _influenceManager.getDistPos(gridPos.x, gridPos.y);

        do
        {
            var index =  random.Next(InfluenceMapManager.CardinalDirections.Length);
            var pos = InfluenceMapManager.CardinalDirections[index] * 3;

            tilePos = _groundTilemap.WorldToCell(transform.position);
            tilePos.x += pos.x;
            tilePos.y += pos.y;

        } while (!_groundTilemap.HasTile(tilePos));
        
        var newWorldPos = _groundTilemap.CellToWorld(tilePos);
        return newWorldPos;
    }
    private Vector3 GetNextPosition()
    {
        var gridPos = _influenceManager.getGridPositionFromGroundMap(transform.position);
        var gridObj = _influenceManager.getDistPos(gridPos.x, gridPos.y);

        var goodPositions = new List<Vector3>(InfluenceMapManager.CardinalDirections.Length);
        
        foreach (var dir in InfluenceMapManager.CardinalDirections)
        {
            var newDir = dir * 3;
            var newGridObj = _influenceManager.getDistPos(gridPos.x + newDir.x, gridPos.y + newDir.y);

            var tilePos = _groundTilemap.WorldToCell(transform.position);
            tilePos.x += newDir.x;
            tilePos.y += newDir.y;
            
            if (newGridObj == null || !_groundTilemap.HasTile(tilePos)) continue;
            
            if (newGridObj.GetDist() != 0 && newGridObj.GetDist() <= gridObj.GetDist())
            {
                return _groundTilemap.CellToWorld(tilePos);
                //goodPositions.Add(_groundTilemap.CellToWorld(tilePos));
            }
        }

        //if (goodPositions.Count == 0)
        return GetRandomWorldPosNearby();
            //goodPositions.Add(transform.position);
        
        // var random = new Random();
        // var index =  random.Next(goodPositions.Count);
        // return goodPositions[index];
        
    }
    private void Update()
    {
        _frameCounter++;
        var position = transform.position;
        
        var curTMapPos = _groundTilemap.WorldToCell(position);
        var distGridPos = _influenceManager.getDistPos(position);

        if (_frameCounter % recalcTime == 0)
        {
            //if (distGridPos.GetDist() != 0)
            _destination = GetNextPosition();
            
            // else if (_destination == new Vector3(0,0,0) || distGridPos.GetDist() == 0)
            // {
            //     _destination = GetRandomWorldPosNearby();
            // }
            
            _frameCounter = 0;
        }
        
        _prevTmapPos = curTMapPos;
    }
    private void FixedUpdate()
    {
        _controller.Move(_destination, 0);
    }
}