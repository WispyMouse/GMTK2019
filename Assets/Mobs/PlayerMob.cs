using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMob : MonoBehaviour
{
    const float MovementSpeed = 6.5f;

    void Update()
    {
        if (PhaseManager.CurrentGameState == GameState.Movement)
        {
            HandleMovement();
        }
    }

    void HandleMovement()
    {
        Vector3 movementInput = Vector3.zero;

        if (Input.GetKey(KeyCode.D))
        {
            movementInput.x = 1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movementInput.x = -1f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            movementInput.z = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movementInput.z = -1f;
        }

        if (movementInput == Vector3.zero)
        {
            return;
        }

        movementInput.Normalize();

        transform.position = transform.position + movementInput * Time.deltaTime * MovementSpeed;
    }
}
