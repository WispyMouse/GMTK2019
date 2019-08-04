using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlowyExplosionHudText : MonoBehaviour
{
    public Text TextToGlow;
    float ColorPongTime { get; set; } = 1.8f;
    public Color ColorOne;
    public Color ColorTwo;
    float CurColorTime { get; set; }

    private void Update()
    {
        CurColorTime += Time.deltaTime;
        TextToGlow.color = Color.Lerp(ColorOne, ColorTwo, Mathf.PingPong(CurColorTime, ColorPongTime) / ColorPongTime);
    }
}
