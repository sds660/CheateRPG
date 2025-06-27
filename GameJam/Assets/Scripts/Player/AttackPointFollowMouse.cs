using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPointFollowMouse : MonoBehaviour
{
    private GameObject atkPoint;
    private MouseInput _mouseInput;

    private float x = 0.6f;
    private float y = 0.5f;

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
        atkPoint = transform.GetChild(0).gameObject;
        _mouseInput.Controls.MousePosition.performed += _ => FollowMouse();
    }

    private void FollowMouse()
    {
        Vector2 mousePos = _mouseInput.Controls.MousePosition.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 position = transform.position;
        var direction = Vector3.Normalize(mousePos - position);

        Vector3 tmp = new Vector3(position.x + direction.x/2, position.y + direction.y/2);
        
        atkPoint.transform.position = tmp;
    }
}
