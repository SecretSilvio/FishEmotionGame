using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Game Settings")]
    [SerializeField] private float pointInterval = 1f;
    [SerializeField] private int score = 0;

    [Header("UI References")]
    private int fishInCircle = 0;
    private Coroutine scoreCoroutine;
    private GameObject scoreText;


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        score = 0;

        scoreText = GameObject.Find("Score Text");
    }

    public void Update()
    {
        UpdateUI();
    }

    public void StartGame()
    {
        Debug.Log("Game Started!");
    }

    public void EndGame()
    {
        Debug.Log("Game Over!");
    }

    public void RestartGame()
    {
        Debug.Log("Game Restarted!");
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit!");
    }

    public void spawnFish()
    {
        Debug.Log("Fish Spawned!");
    }

    public void despawnFish()
    {
        Debug.Log("Fish Despawned!");
    }

    public void startAddingScore()
    {
        fishInCircle++;
        if (scoreCoroutine == null)
        {
            scoreCoroutine = StartCoroutine(AddScoreOverTime());
        }
        Debug.Log("Fish Entered Circle! Total Fish: " + fishInCircle);
    }

    public void stopAddingScore()
    {
        fishInCircle--;
        if (fishInCircle <= 0)
        {
            fishInCircle = 0;
            if (scoreCoroutine != null)
            {
                StopCoroutine(scoreCoroutine);
                scoreCoroutine = null;
            }
        }
        Debug.Log("Fish Left Circle! Total Fish: " + fishInCircle);
    }

    private void UpdateUI()
    {
        // Update UI here
        scoreText.GetComponent<TMPro.TextMeshProUGUI>().text = "Score: " + score.ToString("D4");
    }

    private IEnumerator AddScoreOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(pointInterval);
            score += fishInCircle;
            Debug.Log("Score: " + score);
        }
    }
}