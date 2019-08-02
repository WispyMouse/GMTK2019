using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMob : MonoBehaviour
{
    const float MovementSpeed = 6.5f;
    const float ExhaustionMovementSpeed = .25f;

    public PhaseManager PhaseManagerInstance;
    float HurtAnimationTime { get; } = .6f;
    float HurtBlinkInterval { get; } = .2f;
    float CurHurtTime { get; set; } = 0;

    public float PlayerMaxHealth { get; } = 100f;
    public float PlayerHealth { get; private set; }
    public SpriteRenderer PlayerSpriteRenderer;
    public Sprite PlayerWizardDefeatedSprite;
    public Sprite PlayerWizardExhaustedSprite;

    private void Awake()
    {
        PlayerHealth = PlayerMaxHealth;
    }

    void Update()
    {
        switch (PhaseManager.CurrentGameState)
        {
            case GameState.Movement:
            case GameState.Exhaustion:
                HandleMovement();
                HandleHurtTime();
                break;
            default:
                PlayerSpriteRenderer.enabled = true;
                break;
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

        float curMovementSpeed = MovementSpeed;

        if (PhaseManager.CurrentGameState == GameState.Exhaustion)
        {
            curMovementSpeed = ExhaustionMovementSpeed;
        }

        transform.position = transform.position + movementInput * Time.deltaTime * curMovementSpeed;
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
        if (PhaseManager.CurrentGameState == GameState.Movement)
        {
            PlayerHealth = Mathf.Max(PlayerHealth - amount, 0);
            CurHurtTime = HurtAnimationTime;

            if (PlayerHealth <= 0)
            {
                PlayerSpriteRenderer.sprite = PlayerWizardDefeatedSprite;
                PhaseManagerInstance.PlayerIsDefeated();
            }
        }
        else if(PhaseManager.CurrentGameState == GameState.Exhaustion)
        {
            PlayerHealth = 0;
            PlayerSpriteRenderer.sprite = PlayerWizardDefeatedSprite;
            PhaseManagerInstance.PlayerIsDefeated();
        }
    }

    public void ExhaustionState()
    {
        PlayerSpriteRenderer.sprite = PlayerWizardExhaustedSprite;
    }
}
