using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSounds : MonoBehaviour
{
    public AudioSource[] audioSources; // Array of AudioSources
    public float minInterval = 2f; // Minimum time between plays
    public float maxInterval = 10f; // Maximum time between plays

    private void Start()
    {
        if (audioSources.Length > 0)
        {
            StartCoroutine(PlayRandomAudioAtIntervals());
        }
    }

    private IEnumerator PlayRandomAudioAtIntervals()
    {
        while (true)
        {
            // Wait for a random interval
            float randomInterval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(randomInterval);

            // Choose a random AudioSource and play it
            AudioSource randomAudioSource = audioSources[Random.Range(0, audioSources.Length)];
            randomAudioSource.Play();
        }
    }
}
