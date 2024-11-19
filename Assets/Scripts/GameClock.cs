using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameClock : MonoBehaviour
{
    public TextMeshProUGUI clockText;
    public float roundTime = 60;
    [SerializeField] private float currentTime = 0;
    private bool isRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        clockText.text = roundTime.ToString();
        StartClock();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            currentTime -= Time.deltaTime;
            clockText.text = Mathf.Round(currentTime).ToString();
            if (currentTime <= 0)
            {
                isRunning = false;
                clockText.text = "Time's Up!";
            }
        }
    }

    public void StartClock()
    {
        currentTime = roundTime;
        isRunning = true;
    }

    public void StopClock()
    {
        isRunning = false;
    }
}
