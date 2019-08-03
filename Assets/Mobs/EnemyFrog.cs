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
    public float FullJumpTime;
    public float RisingSpeed;
    public float FallingSpeed;
    float CurPhaseTime { get; set; } = 0f;
    AttackPattern AttackStage { get; set; } = AttackPattern.Rest;

    private void Start()
    {
        FrogShadow = Instantiate(FrogShadowPF);
        FrogShadow.transform.position = transform.position;
    }

    private void Update()
    {
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
                CurPhaseTime = 0;
                AttackStage = AttackPattern.Charge;
                EnemySprite.sprite = FrogCharge;
            }
        }
        else if (AttackStage == AttackPattern.Charge)
        {
            if (CurPhaseTime > ChargeTime)
            {
                CurPhaseTime = 0;
                AttackStage = AttackPattern.Jump;
                EnemySprite.sprite = FrogJumping;
                EnemyCanHurt = false;
            }
        }
        else if (AttackStage == AttackPattern.Jump)
        {
            transform.position = transform.position + Vector3.up * Time.deltaTime * RisingSpeed;

            if (CurPhaseTime > FullJumpTime / 2f)
            {
                CurPhaseTime = 0;
                AttackStage = AttackPattern.Land;
                EnemySprite.sprite = FrogFalling;

                FrogShadow.transform.position = PlayerMobInstance.transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                transform.position = FrogShadow.transform.position + Vector3.up * FallingSpeed * FullJumpTime / 2f;
                EnemyCanHurt = true;
            }
        }
        else if (AttackStage == AttackPattern.Land)
        {
            transform.position = Vector3.MoveTowards(transform.position, FrogShadow.transform.position, Time.deltaTime * FallingSpeed);

            if (CurPhaseTime > FullJumpTime)
            {
                CurPhaseTime = 0;
                AttackStage = AttackPattern.Rest;
                EnemySprite.sprite = NeutralSprite;
            }
        }
    }
}
