using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public Button AttachedButton;
    public Text LevelNameText;
    public GameLevel RepresentedGameLevel { get; private set; }
    public MainMenuControl MainMenuControlInstance;
    public Text SecondLabelText;

    public void SetGameLevel(GameLevel toSet)
    {
        RepresentedGameLevel = toSet;
        this.AttachedButton.onClick.AddListener(() => { MainMenuControlInstance.LevelSelected(RepresentedGameLevel); });
        this.LevelNameText.text = toSet.LevelName.Substring(0, toSet.LevelName.IndexOf("\n"));

        if (!toSet.Accessible)
        {
            AttachedButton.interactable = false;
            SecondLabelText.text = "[locked]";
        }
        else
        {
            if (!toSet.Cleared)
            {
                SecondLabelText.text = "[next!]";
            }
            else
            {
                SecondLabelText.text = "Explosion Score: " + toSet.ExplosionScore.ToString();
            }
        }
    }
}
