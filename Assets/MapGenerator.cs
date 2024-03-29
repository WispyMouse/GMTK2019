﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public PlayerMob PlayerMobInstance;

    public GameObject FloorPF;
    public GameObject WallPF;
    public GameObject TallWallPF;
    public GameObject ExplosionStaffPF;
    public GameObject WaterPF;

    public EnemyCrab EnemyCrabPF;
    public EnemyCrab GiantEnemyCrabPF;
    public EnemyFrog EnemyFrogPF;
    public EnemyFrog GiantEnemyFrogPF;
    public EnemySnail EnemySnailPF;

    public EnemyPlatypus PlatypusPF;

    public GameObject WASDRunePF;
    public GameObject RightClickRunePF;
    public GameObject GetRunePF;
    public GameObject ExplosionRunePF;

    public void GenerateMap(GameLevel toGenerate)
    {
        Vector3 playerSpawnLocation = Vector3.zero;

        Dictionary<Color, GameObject> spawnDictionary = new Dictionary<Color, GameObject>();
        spawnDictionary.Add(new Color(0f, 0f, 0f, 1f), WallPF);
        spawnDictionary.Add(new Color(127f / 255f, 63f / 255f, 91f / 255f, 1f), TallWallPF);

        spawnDictionary.Add(new Color(91f / 255f, 127f / 255f, 0, 1f), WASDRunePF);
        spawnDictionary.Add(new Color(0, 127f / 255f, 14f / 255f, 1f), GetRunePF);
        spawnDictionary.Add(new Color(0, 127f / 255f, 127f / 255f, 1f), RightClickRunePF);
        spawnDictionary.Add(new Color(0, 74f / 255f, 127f / 255f, 1f), ExplosionRunePF);

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
                else if (ColorAtPixel == new Color(63f / 255f, 127f / 255f, 127f / 255f))
                {
                    Instantiate(WaterPF, position, Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(FloorPF, position, Quaternion.identity, transform);

                    if (ColorAtPixel == Color.white)
                    {
                        // this space intentionally left blank
                        // we already made a floor here
                    }
                    else if (ColorAtPixel == new Color(72f / 255f, 0, 255f / 255f, 1f))
                    {
                        GameObject explosionStaff = Instantiate(ExplosionStaffPF, position, Quaternion.identity, transform);
                        PlayerMobInstance.ExplosionStaffInstance = explosionStaff;
                    }
                    else if (ColorAtPixel == new Color(255f / 255f, 106 / 255f, 0, 1f))
                    {
                        EnemyCrab newCrab = Instantiate(EnemyCrabPF, position, Quaternion.identity, transform);
                        newCrab.PlayerMobInstance = PlayerMobInstance;
                    }
                    else if(ColorAtPixel == new Color(127f / 255f, 51f / 255f, 0, 1f))
                    {
                        EnemyCrab newCrab = Instantiate(GiantEnemyCrabPF, position, Quaternion.identity, transform);
                        newCrab.PlayerMobInstance = PlayerMobInstance;
                    }
                    else if (ColorAtPixel == new Color(214 / 255f, 127/ 255f, 255f / 255f, 1f))
                    {
                        EnemyPlatypus newPlatypus = Instantiate(PlatypusPF, position, Quaternion.identity, transform);
                        newPlatypus.PlayerMobInstance = PlayerMobInstance;
                    }
                    else if (ColorAtPixel == new Color(76f / 255f, 255f / 255f, 0, 1f))
                    {
                        EnemyFrog newFrog = Instantiate(EnemyFrogPF, position, Quaternion.identity, transform);
                        newFrog.PlayerMobInstance = PlayerMobInstance;
                    }
                    else if (ColorAtPixel == new Color(0, 255f / 255f, 144f / 255f, 1f))
                    {
                        EnemyFrog newFrog = Instantiate(GiantEnemyFrogPF, position, Quaternion.identity, transform);
                        newFrog.PlayerMobInstance = PlayerMobInstance;
                    }
                    else if (ColorAtPixel == new Color(255f / 255f, 0 / 255f, 110f / 255f, 1f))
                    {
                        EnemySnail newSnail = Instantiate(EnemySnailPF, position, Quaternion.identity, transform);
                        newSnail.PlayerMobInstance = PlayerMobInstance;
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
