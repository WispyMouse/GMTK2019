using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMob : MonoBehaviour
{
    const float MovementSpeed = 6.5f;

    public PhaseManager PhaseManagerInstance;
    float HurtAnimationTime { get; } = .6f;
    float HurtBlinkInterval { get; } = .2f;
    float CurHurtTime { get; set; } = 0;

    public float PlayerMaxHealth { get; } = 100f;
    public float PlayerHealth { get; private set; }
    public SpriteRenderer PlayerSpriteRenderer;

    private void Awake()
    {
        PlayerHealth = PlayerMaxHealth;
    }

    void Update()
    {
        if (PhaseManager.CurrentGameState == GameState.Movement)
        {
            HandleMovement();
        }

        HandleHurtTime();
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

    void HandleHurtTime()
    {
        if (CurHurtTime > 0)
        {
            float curBlinkTime = CurHurtTime % HurtBlinkInterval * 2f;

            if (curBlinkTime < HurtBlinkInterval)
            {
                PlayerSpriteRenderer.enabled = false;
            }
            else
            {
                PlayerSpriteRenderer.enabled = true;
            }

            CurHurtTime = Mathf.Max(CurHurtTime - Time.deltaTime, 0);
        }
        else
        {
            PlayerSpriteRenderer.enabled = true;
        }
    }

    public bool InHurtTime
    {
        get
        {
            return CurHurtTime > 0;
        }
    }

    public void TakeDamage(float amount)
    {
        PlayerHealth = Mathf.Max(PlayerHealth - amount, 0);
        CurHurtTime = HurtAnimationTime;
    }
}
