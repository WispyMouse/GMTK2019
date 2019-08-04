using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMob : Mob
{
    const float MovementSpeed = 6.25f;
    const float FinalBossExhaustionSpeed = .9f;
    const float ExhaustionMovementSpeed = .35f;

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
    public Sprite PlayerWizardCastingSprite;
    public LayerMask ExplosionStaffMask;
    public LayerMask GemOfBraggingRightsMask;

    float CurWalkTime { get; set; } = 0;
    float TimeForWalkSound { get; } = .375f;
    public AudioClip WalkSound;
    public AudioClip HurtSound;
    public AudioClip DefeatSound;
    public AudioClip GemOfBraggingRightsSound;
    public AudioClip ExhaustionSound;
    public AudioClip StaffGet;

    public GameObject ExplosionStaffInstance { get; set; }
    public Transform GuideArrow;
    public GameObject GemOfBraggingRightsInstance { get; set; }

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

        if (ExplosionStaffInstance != null || GemOfBraggingRightsInstance != null)
        {
            Vector3 target;

            if (GemOfBraggingRightsInstance != null)
            {
                target = GemOfBraggingRightsInstance.transform.position;
            }
            else
            {
                target = ExplosionStaffInstance.transform.position;
            }

            GuideArrow.transform.rotation = Quaternion.Euler(90f, Vector3.SignedAngle(target - transform.position, Vector3.right, Vector3.down), 0);
            GuideArrow.gameObject.SetActive(true);
        }
        else
        {
            GuideArrow.gameObject.SetActive(false);
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
            if (MainMenuControl.SelectedLevel.FinalBossChapter)
            {
                curMovementSpeed = FinalBossExhaustionSpeed;
            }
            else
            {
                curMovementSpeed = ExhaustionMovementSpeed;
            }
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

            if (PhaseManager.CurrentGameState == GameState.Exhaustion)
            {
                SoundPlayer.PlayBoomingSound(WalkSound, .45f);
            }
            else
            {
                SoundPlayer.PlayPitchAdjustedSound(WalkSound, .45f);
            }
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
                SoundPlayer.PlayPitchAdjustedSound(DefeatSound);
            }
            else
            {
                SoundPlayer.PlayPitchAdjustedSound(HurtSound);
            }
        }
        else if(PhaseManager.CurrentGameState == GameState.Exhaustion)
        {
            PlayerHealth = 0;
            PlayerSpriteRenderer.sprite = PlayerWizardExhaustedDefeatedSprite;
            PhaseManagerInstance.PlayerIsDefeated();
            SoundPlayer.PlayPitchAdjustedSound(DefeatSound);
        }

        
    }

    public void ExhaustionState()
    {
        PlayerSpriteRenderer.sprite = PlayerWizardExhaustedSprite;
        SoundPlayer.PlaySound(ExhaustionSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ExplosionStaffMask == (ExplosionStaffMask | (1 << other.gameObject.layer)))
        {
            if (PhaseManager.CurrentGameState != GameState.Movement)
            {
                return;
            }

            PlayerSpriteRenderer.sprite = PlayerWizardWithStaffSprite;
            Destroy(other.gameObject);
            SoundPlayer.PlayPitchAdjustedSound(StaffGet);
            PhaseManagerInstance.PlayerPicksUpStaff();
        }
        else if (GemOfBraggingRightsMask == (GemOfBraggingRightsMask | (1 << other.gameObject.layer)))
        {
            Destroy(other.gameObject);
            SoundPlayer.PlaySound(GemOfBraggingRightsSound);
            PhaseManagerInstance.PlayerPicksUpGemOfBraggingRights();
        }
    }

    public void CastingState()
    {
        PlayerSpriteRenderer.sprite = PlayerWizardCastingSprite;
    }
}
