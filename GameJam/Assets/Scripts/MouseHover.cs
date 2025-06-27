using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseHover : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    private MouseInput _mouseInput;

    private void Awake()
    {
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

    // Start is called before the first frame update
    void Start()
    {
        _mouseInput.Controls.MousePosition.performed += _ => FollowMouse();
    }
    
    void FollowMouse()
    {
        Vector2 mousePos = _mouseInput.Controls.MousePosition.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        
        Vector3Int gridPos = tileMap.WorldToCell(mousePos);

        if (tileMap.HasTile(gridPos))
        {
            transform.position = tileMap.GetCellCenterWorld(gridPos) + new Vector3(0, 0.25f, 0);
        }
    }
}
