﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControl : MonoBehaviour
{
    public static bool ShowLevelSelect { get; set; } = false;
    public static GameLevel SelectedLevel { get; set; }
    public Camera ActiveCamera;

    public Color ColorOne;
    public Color ColorTwo;
    float CurColorTime { get; set; }

    float ColorPongTime { get; set; } = 8f;
    float RotationSpeed { get; set; } = 10f;

    public Transform FirstMenu;
    public Transform LevelSelection;
    public Transform LevelsParent;
    public LevelButton LevelButtonTemplate;
    public LevelManager LevelManagerInstance;

    public List<Transform> RuneCircles;

    private void Awake()
    {
        FirstMenu.gameObject.SetActive(!ShowLevelSelect);
        LevelSelection.gameObject.SetActive(ShowLevelSelect);

        EstablishLevelSelect();
    }

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
        FirstMenu.gameObject.SetActive(false);
        LevelSelection.gameObject.SetActive(true);
    }

    public void LevelSelected(GameLevel selected)
    {
        SelectedLevel = selected;
        SceneManager.LoadScene(1);
    }

    void EstablishLevelSelect()
    {
        LevelButtonTemplate.gameObject.SetActive(false);

        for (int ii = 0; ii < LevelManagerInstance.GameLevelCount; ii++)
        {
            LevelButton newButton = Instantiate(LevelButtonTemplate, LevelsParent);
            newButton.SetGameLevel(LevelManagerInstance.GetLevel(ii));
            newButton.gameObject.SetActive(true);
        }
    }
}
