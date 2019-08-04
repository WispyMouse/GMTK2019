using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlatypus : EnemyCrab
{
    public GameObject GemOfBraggingRights;

    protected override void DropItem()
    {
        Instantiate(GemOfBraggingRights, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
    }
}
