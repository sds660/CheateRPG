using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUnhide : MonoBehaviour
{
    [SerializeField] private float flashTime;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {  
        StartCoroutine("FlashObject");
    }
    
    IEnumerator FlashObject()
    {
        while (true) {
            yield return (new WaitForSeconds(flashTime));
            //gameObject.SetActive(true);
            _spriteRenderer.enabled = true;
            yield return (new WaitForSeconds(flashTime));
            //gameObject.SetActive(false);
            _spriteRenderer.enabled = false;
        }
    }
}
