using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuControl : MonoBehaviour
{
    public static bool ShowLevelSelect { get; set; } = false;
    public static GameLevel SelectedLevel { get; set; }
    public Camera ActiveCamera;

    public Color ColorOne;
    public Color ColorTwo;
    float CurColorTime { get; set; }

    public Color MintColorOne;
    public Color MintColorTwo;
    float CurMintColorTime;
    float MintColorPongTime { get; set; } = 1f;
    public Text CreditsText;

    float ColorPongTime { get; set; } = 8f;
    float RotationSpeed { get; set; } = 10f;

    public Transform FirstMenu;
    public Transform LevelSelection;
    public Transform LevelsParent;
    public LevelButton LevelButtonTemplate;
    public LevelManager LevelManagerInstance;

    public List<Transform> RuneCircles;

    public AudioClip MenuSelectSound;

    private void Awake()
    {
        FirstMenu.gameObject.SetActive(!ShowLevelSelect);
        LevelSelection.gameObject.SetActive(ShowLevelSelect);

        EstablishLevelSelect();
    }

    private void Update()
    {
        CurColorTime += Time.deltaTime;
        CurMintColorTime += Time.deltaTime;
        ActiveCamera.backgroundColor = Color.Lerp(ColorOne, ColorTwo, Mathf.PingPong(CurColorTime, ColorPongTime) / ColorPongTime);

        foreach (Transform circle in RuneCircles)
        {
            circle.transform.rotation = Quaternion.Euler(0, 0, circle.transform.rotation.eulerAngles.z + Time.deltaTime * RotationSpeed);
        }

        Color curColor = Color.Lerp(MintColorOne, MintColorTwo, Mathf.PingPong(CurMintColorTime, MintColorPongTime) / MintColorPongTime);
        CreditsText.text = $"Made by <color=#{ColorUtility.ToHtmlStringRGB(curColor)}>Mint</color> for GMTK 2019\n\n@WispyMouse";

        if (Input.GetKeyDown(KeyCode.Home))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void StartGameButton()
    {
        FirstMenu.gameObject.SetActive(false);
        LevelSelection.gameObject.SetActive(true);
        SoundPlayer.PlaySound(MenuSelectSound);
    }

    public void StartGameWithoutSoundButton()
    {
        SoundPlayer.MuteSound();
        StartGameButton();
    }

    public void LevelSelected(GameLevel selected)
    {
        SelectedLevel = selected;
        SceneManager.LoadScene(1);
        SoundPlayer.PlaySound(MenuSelectSound);
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
