using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFrog : EnemyMob
{
    public enum AttackPattern { Rest, Charge, Jump, Land }

    public PlayerMob PlayerMobInstance { get; set; }
    public GameObject FrogShadowPF;

    public Sprite FrogCharge;
    public Sprite FrogJumping;
    public Sprite FrogFalling;
    public GameObject FrogShadow { get; set; }

    public float RestTime;
    public float ChargeTime;
    public float RisingTime;
    public float FallingTime;
    public float RisingSpeed;
    public float FallingSpeed;
    float CurPhaseTime { get; set; } = 0f;
    AttackPattern AttackStage { get; set; } = AttackPattern.Rest;

    public AudioClip FrogJumpUpSound;
    public AudioClip FrogLandSound;

    private void Start()
    {
        FrogShadow = Instantiate(FrogShadowPF);
        FrogShadow.transform.position = transform.position;
        CurPhaseTime = Random.Range(0f, .15f);
    }

    private void Update()
    {
        if (!EnemyActive)
        {
            FrogShadow.SetActive(false);
        }

        switch (PhaseManager.CurrentGameState)
        {
            case GameState.Movement:
            case GameState.Defeated:
            case GameState.Exhaustion:
                HandleMovement();
                return;
        }
    }

    void HandleMovement()
    {
        CurPhaseTime += Time.deltaTime;

        if (AttackStage == AttackPattern.Rest)
        {
            if (CurPhaseTime > RestTime)
            {
                CurPhaseTime -= RestTime;
                AttackStage = AttackPattern.Charge;
                EnemySprite.sprite = FrogCharge;
            }
        }
        else if (AttackStage == AttackPattern.Charge)
        {
            if (CurPhaseTime > ChargeTime)
            {
                CurPhaseTime -= ChargeTime;
                AttackStage = AttackPattern.Jump;
                EnemySprite.sprite = FrogJumping;

                if (IsGiant)
                {
                    SoundPlayer.PlayBoomingSound(FrogJumpUpSound, .25f);
                }
                else
                {
                    SoundPlayer.PlayPitchAdjustedSound(FrogJumpUpSound, .25f);
                }
            }
        }
        else if (AttackStage == AttackPattern.Jump)
        {
            transform.position = transform.position + Vector3.up * Time.deltaTime * RisingSpeed;

            if (CurPhaseTime > RisingTime)
            {
                CurPhaseTime -= RisingTime;
                AttackStage = AttackPattern.Land;
                EnemySprite.sprite = FrogFalling;

                Vector3 randomCircle = Random.onUnitSphere;
                FrogShadow.transform.position = PlayerMobInstance.transform.position + new Vector3(randomCircle.x * .5f, 0, randomCircle.z * .5f);
                transform.position = FrogShadow.transform.position + Vector3.up * FallingSpeed * FallingTime;

                if (IsGiant)
                {
                    SoundPlayer.PlayBoomingSound(FrogLandSound, .25f);
                }
                else
                {
                    SoundPlayer.PlayPitchAdjustedSound(FrogLandSound, .25f);
                }
            }
        }
        else if (AttackStage == AttackPattern.Land)
        {
            transform.position = Vector3.MoveTowards(transform.position, FrogShadow.transform.position, Time.deltaTime * FallingSpeed);

            if (CurPhaseTime > FallingTime)
            {
                CurPhaseTime -= FallingTime;
                AttackStage = AttackPattern.Rest;
                EnemySprite.sprite = NeutralSprite;
            }
        }
    }
}
