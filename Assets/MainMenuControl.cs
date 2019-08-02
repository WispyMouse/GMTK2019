using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControl : MonoBehaviour
{
    public Camera ActiveCamera;

    public Color ColorOne;
    public Color ColorTwo;
    float CurColorTime { get; set; }

    float ColorPongTime { get; set; } = 8f;
    float RotationSpeed { get; set; } = 10f;

    public List<Transform> RuneCircles;

    private void Update()
    {
        CurColorTime += Time.deltaTime;
        ActiveCamera.backgroundColor = Color.Lerp(ColorOne, ColorTwo, Mathf.PingPong(CurColorTime, ColorPongTime) / ColorPongTime);

        foreach (Transform circle in RuneCircles)
        {
            circle.transform.rotation = Quaternion.Euler(0, 0, circle.transform.rotation.eulerAngles.z + Time.deltaTime * RotationSpeed);
        }
    }

    public void StartGameButton()
    {
        SceneManager.LoadScene(1);
    }
}
