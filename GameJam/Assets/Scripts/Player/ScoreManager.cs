using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private GameObject _scoreText;
    private float score = 0f;

    private void Awake()
    {
        _scoreText = GameObject.Find("Player UI/Score");
    }

    public void AddPoints(float points)
    {
        score += points;
        UpdateScore();
    }

    public void UpdateScore()
    {
        _scoreText.GetComponent<TMPro.TextMeshProUGUI>().text = "Score: " + (int)score;
    }

    public float GetScore()
    {
        return score;
    }
}
