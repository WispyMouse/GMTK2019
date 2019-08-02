using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobGenerator : MonoBehaviour
{
    public PlayerMob PlayerMobInstance;
    public EnemyCrab EnemyCrabPF;

    List<EnemyCrab> EnemyCrabs { get; set; } = new List<EnemyCrab>();

    public void CreateEnemyCrab(Vector3 position)
    {
        EnemyCrab newCrab = Instantiate(EnemyCrabPF, position, Quaternion.identity, transform);
        newCrab.PlayerMobInstance = PlayerMobInstance;
        EnemyCrabs.Add(newCrab);
    }
}
