using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<GameLevel> GameLevels;

    public GameLevel GetLevel(int index)
    {
        if (index < 0)
        {
            return null;
        }

        if (index > GameLevelCount)
        {
            return null;
        }

        GameLevel toGet = GameLevels[index];

        toGet.Cleared = (PlayerPrefs.GetInt(index.ToString() + "Clear", 0) == 1);
        toGet.Accessible = index == 0 || (PlayerPrefs.GetInt((index - 1).ToString() + "Clear", 0) == 1);
        toGet.ExplosionScore = PlayerPrefs.GetInt(toGet.LevelName + "ExplosionScore", 0);
        toGet.LevelIndex = index;

        return toGet;
    }

    public int GameLevelCount
    {
        get
        {
            return GameLevels.Count;
        }
    }

    public void ClearLevel(GameLevel toClear, int explosionScore)
    {
        PlayerPrefs.SetInt(toClear.LevelIndex + "Clear", 1);

        int previousScore = PlayerPrefs.GetInt(toClear.LevelName + "ExplosionScore", 0);

        if (explosionScore > previousScore)
        {
            PlayerPrefs.SetInt(toClear.LevelName + "ExplosionScore", explosionScore);
        }
    }
}
