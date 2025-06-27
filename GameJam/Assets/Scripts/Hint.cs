using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hint : MonoBehaviour
{
    [SerializeField] private string hintText;
    private Vector3 startPos;
    private int direction = 1;
    private float minY, maxY;
    private GameObject popup;

    private void Awake()
    {
        Transform canvasTransform = GameObject.Find("Player UI").transform;
        popup = Instantiate((GameObject)Resources.Load("CheatText"), canvasTransform);
        popup.transform.SetParent(canvasTransform);
        popup.GetComponent<TMPro.TextMeshProUGUI>().text = hintText;
        
        TMP_FontAsset newFont = Resources.Load("Pixelbroidery-0n0G SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
        popup.GetComponent<TextMeshProUGUI>().font = newFont;
        popup.SetActive(false);
    }

    private void Start()
    {
        startPos = transform.position;
        minY = startPos.y - 0.075f;
        maxY = startPos.y + 0.075f;
    }

    private void Update()
    {
        transform.position += new Vector3(0,0.001f,0) * direction;
        if (transform.position.y > maxY)
        {
            direction = -1;
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
        }
        else if (transform.position.y < minY)
        {
            direction = 1;
            transform.position = new Vector3(transform.position.x, minY, transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            popup.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            popup.SetActive(false);
        }
    }
}