using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public Transform ToWobble;
    float CurWobbleTime { get; set; }
    float WobbleTime { get; } = 1.25f;
    float MaxHeight { get; } = -.05f;
    float WobbleHeight { get; } = -.2f;

    private void Update()
    {
        CurWobbleTime += Time.deltaTime;

        ToWobble.localPosition = Vector3.up * ((Mathf.PingPong(CurWobbleTime, WobbleTime) / WobbleTime * (WobbleHeight - MaxHeight)) + MaxHeight);
    }
}
