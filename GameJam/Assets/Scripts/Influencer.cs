using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class Influencer : MonoBehaviour
{
    [SerializeField] private GameObject influenceMapManagerObject;
    [SerializeField] private int recalcTime;
    [SerializeField] private GameObject tileMapObj;
    
    private Tilemap _groundTilemap;
    private int _frameCounter = 0;
    private Vector3Int _prevTmapPos;
    private InfluenceMapManager _influenceManager;

    private void Awake()
    {
        _influenceManager = influenceMapManagerObject.GetComponent<InfluenceMapManager>();

        _groundTilemap = tileMapObj.transform.Find("Ground Map").GetComponent<Tilemap>();
    }
    private void Start()
    {
        _prevTmapPos = _groundTilemap.WorldToCell(transform.position);
    }
    private void Update()
    {
        _frameCounter++;
        
        var curTMapPos = _groundTilemap.WorldToCell(transform.position);
        
        if (_frameCounter % recalcTime == 0)
        {
            _influenceManager.CalculateDistance(transform.position);
            _frameCounter = 0;
        }
        
        _prevTmapPos = curTMapPos;
    }
}
