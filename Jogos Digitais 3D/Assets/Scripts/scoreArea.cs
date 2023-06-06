using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scoreArea : MonoBehaviour
{
    private ScoreManager scoreManager;
    private bool scoreCounted;
    // Start is called before the first frame update
    void Start()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("score") && !scoreCounted)
        {
            scoreCounted = true;
            scoreManager.AddScore();
        }
    }
}
