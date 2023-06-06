using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    public int scoreCount;
    public void AddScore()
    {
        scoreCount++;
        scoreText.text = scoreCount.ToString();
    }
}
