﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Sprite MapSprite;
    public TerrainGenerator TerrainGeneratorInstance;
    public MobGenerator MobGeneratorInstance;
    public PlayerMob PlayerMobInstance;

    public void GenerateMap()
    {
        Vector3 playerSpawnLocation = Vector3.zero;

        for (int xx = 0; xx < MapSprite.texture.width; xx++)
        {
            for (int yy = 0; yy < MapSprite.texture.height; yy++)
            {
                Vector3 position = new Vector3(xx, 0, yy);
                Color ColorAtPixel = MapSprite.texture.GetPixel(xx, yy);

                if (ColorAtPixel == Color.black)
                {
                    TerrainGeneratorInstance.CreateWall(position);
                }
                else if (ColorAtPixel == new Color(128, 128, 128))
                {
                    // this space intentionally left blank
                }
                else
                {
                    TerrainGeneratorInstance.CreateFloor(position);

                    if (ColorAtPixel == Color.white)
                    {
                        // this space intentionally left blank
                        // we already made a floor here
                    }
                    else if (ColorAtPixel == new Color(255 / 255f, 106 / 255f, 0, 1f))
                    {
                        MobGeneratorInstance.CreateEnemyCrab(position);
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
