using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "level", menuName = "GMTK/Level")]
public class GameLevel : ScriptableObject
{
    public string LevelName;
    public Sprite LevelMap;
    
    public bool Accessible { get; set; }
    public int ExplosionScore { get; set; }
    public bool Cleared { get; set; }
    public int LevelIndex { get; set; }
}
