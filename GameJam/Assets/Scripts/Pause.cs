using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _healthBar;
    
    private bool _isPaused = false;

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
    private void Start()
    {
        _mouseInput.Controls.Pause.performed += _ => DoPause(); 
    }
    public void DoPause()
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            Time.timeScale = 0;
            _healthBar.SetActive(false);
            _pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            _pauseMenu.SetActive(false);
            _healthBar.SetActive(true);
        }
    }
}