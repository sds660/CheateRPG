using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private string goalText;
    [SerializeField] private float goalPoints;
    [SerializeField] private LevelLoader _levelLoader;
    private GameObject popup;

    private void Awake()
    {
        Transform canvasTransform = GameObject.Find("Player UI").transform;
        popup = Instantiate((GameObject)Resources.Load("CheatText"), canvasTransform);
        popup.transform.SetParent(canvasTransform);
        popup.GetComponent<TMPro.TextMeshProUGUI>().text = goalText;

        TMP_FontAsset newFont = Resources.Load("Pixelbroidery-0n0G SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
        popup.GetComponent<TextMeshProUGUI>().font = newFont;
        popup.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<ScoreManager>().GetScore() >= goalPoints)
            {
                _levelLoader.LoadNextLevel();
            }
            else
            {
                popup.SetActive(true);
            }
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
