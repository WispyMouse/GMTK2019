using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMob : Mob
{
    const float MovementSpeed = 6.5f;
    const float ExhaustionMovementSpeed = .25f;

    public PhaseManager PhaseManagerInstance;
    float HurtAnimationTime { get; } = .9f;
    float HurtBlinkInterval { get; } = .3f;
    float CurHurtTime { get; set; } = 0;

    public int PlayerHealth { get; private set; } = 3;
    public SpriteRenderer PlayerSpriteRenderer;
    public Sprite PlayerWizardDefeatedSprite;
    public Sprite PlayerWizardExhaustedSprite;
    public Sprite PlayerWizardExhaustedDefeatedSprite;
    public Sprite PlayerWizardWithStaffSprite;
    public LayerMask ExplosionStaffMask;

    float CurWalkTime { get; set; } = 0;
    float TimeForWalkSound { get; } = .375f;
    public AudioClip WalkSound;
    public AudioClip HurtSound;

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
            CurWalkTime = Mathf.Max(CurWalkTime, TimeForWalkSound * .75f);
            return;
        }

        movementInput.Normalize();

        float curMovementSpeed = MovementSpeed;

        if (PhaseManager.CurrentGameState == GameState.Exhaustion)
        {
            curMovementSpeed = ExhaustionMovementSpeed;
        }

        Walk(movementInput * Time.deltaTime * curMovementSpeed);

        if (PhaseManager.CurrentGameState == GameState.Exhaustion)
        {
            CurWalkTime += Time.deltaTime * .25f;
        }
        else
        {
            CurWalkTime += Time.deltaTime;
        }

        if (CurWalkTime > TimeForWalkSound)
        {
            CurWalkTime -= TimeForWalkSound;
            SoundPlayer.PlayPitchAdjustedSound(WalkSound, .25f);
        }
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

    public void TakeDamage(int amount)
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
            PlayerSpriteRenderer.sprite = PlayerWizardExhaustedDefeatedSprite;
            PhaseManagerInstance.PlayerIsDefeated();
        }

        SoundPlayer.PlayPitchAdjustedSound(HurtSound);
    }

    public void ExhaustionState()
    {
        PlayerSpriteRenderer.sprite = PlayerWizardExhaustedSprite;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhaseManager.CurrentGameState != GameState.Movement)
        {
            return;
        }

        if (ExplosionStaffMask != (ExplosionStaffMask | (1 << other.gameObject.layer)))
        {
            return;
        }

        PlayerSpriteRenderer.sprite = PlayerWizardWithStaffSprite;
        Destroy(other.gameObject);

        PhaseManagerInstance.PlayerPicksUpStaff();
    }
}
