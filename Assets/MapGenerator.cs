using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public PlayerMob PlayerMobInstance;

    public GameObject FloorPF;
    public GameObject WallPF;
    public GameObject ExplosionStaffPF;

    public EnemyCrab EnemyCrabPF;

    public GameObject WASDRunePF;
    public GameObject RightClickRunePF;
    public GameObject GetRunePF;
    public GameObject ExplosionRunePF;

    public void GenerateMap(GameLevel toGenerate)
    {
        Vector3 playerSpawnLocation = Vector3.zero;

        Dictionary<Color, GameObject> spawnDictionary = new Dictionary<Color, GameObject>();
        spawnDictionary.Add(new Color(91f / 255f, 127f / 255f, 0, 1f), WASDRunePF);
        spawnDictionary.Add(new Color(0, 127f / 255f, 14f / 255f, 1f), GetRunePF);
        spawnDictionary.Add(new Color(0, 127f / 255f, 127f / 255f, 1f), RightClickRunePF);
        spawnDictionary.Add(new Color(0, 74f / 255f, 127f / 255f, 1f), ExplosionRunePF);
        spawnDictionary.Add(new Color(72f / 255f, 0, 255f / 255f, 1f), ExplosionStaffPF);
        spawnDictionary.Add(new Color(0f, 0f, 0f, 1f), WallPF);

        for (int xx = 0; xx < toGenerate.LevelMap.texture.width; xx++)
        {
            for (int yy = 0; yy < toGenerate.LevelMap.texture.height; yy++)
            {
                Vector3 position = new Vector3(xx, 0, yy);
                Color ColorAtPixel = toGenerate.LevelMap.texture.GetPixel(xx, yy);

                if (ColorAtPixel == new Color(128f / 255f, 128f / 255f, 128f / 255f))
                {
                    // this space intentionally left blank
                }
                else
                {
                    Instantiate(FloorPF, position, Quaternion.identity, transform);

                    if (ColorAtPixel == Color.white)
                    {
                        // this space intentionally left blank
                        // we already made a floor here
                    }
                    else if (ColorAtPixel == new Color(255f / 255f, 106 / 255f, 0, 1f))
                    {
                        EnemyCrab newCrab = Instantiate(EnemyCrabPF, position, Quaternion.identity, transform);
                        newCrab.PlayerMobInstance = PlayerMobInstance;
                    }
                    else if (spawnDictionary.ContainsKey(ColorAtPixel))
                    {
                        Instantiate(spawnDictionary[ColorAtPixel], position, Quaternion.identity, transform);
                    }
                    else if (ColorAtPixel == new Color(127f / 255f, 255f / 255f, 255f / 255f, 1f))
                    {
                        playerSpawnLocation = position;
                    }
                    else
                    {
                        Debug.Log($"Unrecognized color at ({xx}, {yy}): ({ColorAtPixel.r}, {ColorAtPixel.g}, {ColorAtPixel.b})");
                    }
                }
            }
        }

        PlayerMobInstance.transform.position = playerSpawnLocation;
    }
}
