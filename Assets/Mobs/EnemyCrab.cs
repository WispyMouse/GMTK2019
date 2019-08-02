using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCrab : MonoBehaviour
{
    public PlayerMob PlayerMobInstance;
    const float MovementSpeed = 3.5f;

    private void Update()
    {
        if (PhaseManager.CurrentGameState == GameState.Movement)
        {
            HandleMovement();
            return;
        }        
    }

    void HandleMovement()
    {
        Vector3 targetPosition = Vector3.MoveTowards(transform.position, PlayerMobInstance.transform.position, Time.deltaTime * MovementSpeed);
        transform.position = targetPosition;
    }
}
