using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private float destinationOffset;
    private IsoController _controller;

    private MouseInput _mouseInput;
    private Vector3 _destination;

    private void Awake()
    {
        TryGetComponent<IsoController>(out _controller);
        _mouseInput = new MouseInput();
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
        _mouseInput.Controls.Move.performed += _ => MouseClick();
        _destination = transform.position;
    }
    private void MouseClick()
    {
        var mousePos = _mouseInput.Controls.MousePosition.ReadValue<Vector2>();
        if (Camera.main is { }) mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        var gridPos = tileMap.WorldToCell(mousePos);
    
        if (tileMap.HasTile(gridPos))
        {
            //Debug.Log("Tile pos at click: " + gridPos);
            _destination = mousePos;
        }

    }
    private void FixedUpdate()
    {
        _controller.Move(_destination, destinationOffset);
    }
}