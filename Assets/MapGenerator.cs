using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public TerrainGenerator TerrainGeneratorInstance;
    public MobGenerator MobGeneratorInstance;
    public PlayerMob PlayerMobInstance;

    private void Start()
    {
        for (int xx = 1; xx <= 20; xx++)
        {
            for (int zz = 1; zz <= 20; zz++)
            {
                if (xx == 1 || xx == 20 || zz == 1 || zz == 20)
                {
                    TerrainGeneratorInstance.CreateWall(new Vector3(xx, 0, zz));
                }
                else
                {
                    TerrainGeneratorInstance.CreateFloor(new Vector3(xx, 0, zz));
                }
            }
        }

        MobGeneratorInstance.CreateEnemyCrab(new Vector3(4f, 0, 4f));
        MobGeneratorInstance.CreateEnemyCrab(new Vector3(8f, 0, 2.5f));
        MobGeneratorInstance.CreateEnemyCrab(new Vector3(7f, 0, 14f));
        MobGeneratorInstance.CreateEnemyCrab(new Vector3(4f, 0, 18f));
        MobGeneratorInstance.CreateEnemyCrab(new Vector3(17f, 0, 3f));

        PlayerMobInstance.transform.position = new Vector3(10f, 0, 10f);
    }
}
