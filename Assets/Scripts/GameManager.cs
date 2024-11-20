using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Game Settings")]
    [SerializeField] private float pointInterval = 1f;
    [SerializeField] private int score = 0;

    [Header("UI References")]
    private int fishInCircle = 0;
    private Coroutine scoreCoroutine;


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
    }

    public void Update()
    {

    }

    public void StartGame()
    {
        //load the samplescene
        SceneManager.LoadScene("SampleScene");
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
}