using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleController : MonoBehaviour
{
    public float minSize = 0.5f;
    public float maxSize = 2.0f;
    public float speed = 1.0f;
    public float visibleDuration = 2.0f;
    public float goneDuration = 2.0f;
    public float fadeDuration = 1.0f;
    public Color circleColor = Color.blue;

    private SpriteRenderer spriteRenderer;
    private float timer = 0.0f;
    private float screenWidth;
    private float screenHeight;
    private enum State { Visible, Shrinking, Moving, FadingIn, Gone }
    private State currentState;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = circleColor;
        screenWidth = Camera.main.orthographicSize * Camera.main.aspect;
        screenHeight = Camera.main.orthographicSize;
        currentState = State.Visible;
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (currentState)
        {
            case State.Visible:
                if (timer >= visibleDuration)
                {
                    timer = 0.0f;
                    currentState = State.Shrinking;
                }
                break;

            case State.Shrinking:
                float size = Mathf.Lerp(maxSize, minSize, timer / fadeDuration);
                transform.localScale = new Vector3(size, size, 1);
                float alpha = Mathf.Lerp(1, 0, timer / fadeDuration) * 0.75f; // Clamp alpha to 75%
                spriteRenderer.color = new Color(circleColor.r, circleColor.g, circleColor.b, alpha);

                if (timer >= fadeDuration)
                {
                    timer = 0.0f;
                    currentState = State.Moving;
                }
                break;

            case State.Moving:
                float x = Random.Range(-screenWidth + maxSize / 2, screenWidth - maxSize / 2);
                float y = Random.Range(-screenHeight + maxSize / 2, screenHeight - maxSize / 2);
                transform.position = new Vector3(x, y, 0);

                timer = 0.0f;
                currentState = State.Gone;
                break;

            case State.Gone:
                if (timer >= goneDuration)
                {
                    timer = 0.0f;
                    currentState = State.FadingIn;
                }
                break;

            case State.FadingIn:
                float fadeInSize = Mathf.Lerp(minSize, maxSize, timer / fadeDuration);
                transform.localScale = new Vector3(fadeInSize, fadeInSize, 1);
                float fadeInAlpha = Mathf.Lerp(0, 1, timer / fadeDuration) * 0.75f;
                spriteRenderer.color = new Color(circleColor.r, circleColor.g, circleColor.b, fadeInAlpha);

                if (timer >= fadeDuration)
                {
                    timer = 0.0f;
                    currentState = State.Visible;
                }
                break;
        }
    }
}