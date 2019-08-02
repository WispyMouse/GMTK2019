using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobGenerator : MonoBehaviour
{
    public PlayerMob PlayerMobInstance;
    public EnemyCrab EnemyCrabPF;

    public void CreateEnemyCrab(Vector3 position)
    {
        EnemyCrab newCrab = Instantiate(EnemyCrabPF, position, Quaternion.identity, transform);
        newCrab.PlayerMobInstance = PlayerMobInstance;
    }
}
