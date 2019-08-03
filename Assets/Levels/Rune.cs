using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : MonoBehaviour
{
    public SpriteRenderer RuneSpriteRenderer;
    float ColorPongTime { get; set; } = .9f;
    public Color ColorOne;
    public Color ColorTwo;
    float CurColorTime { get; set; }

    private void Update()
    {
        CurColorTime += Time.deltaTime;
        RuneSpriteRenderer.color = Color.Lerp(ColorOne, ColorTwo, Mathf.PingPong(CurColorTime, ColorPongTime) / ColorPongTime);
    }
}
