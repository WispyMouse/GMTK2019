using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject FloorPF;
    public GameObject WallPF;

    public void CreateFloor(Vector3 atPosition)
    {
        Instantiate(FloorPF, atPosition, Quaternion.identity, transform);
    }

    public void CreateWall(Vector3 atPosition)
    {
        Instantiate(WallPF, atPosition, Quaternion.identity, transform);
    }
}
