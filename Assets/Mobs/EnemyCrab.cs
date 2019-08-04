using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCrab : EnemyMob
{
    public enum AttackPattern { Rest, Charge, Attack }
    public PlayerMob PlayerMobInstance { get; set; }
    public float MovementSpeed;

    public Sprite CrabChargeUp;
    public Sprite CrabAttack;

    public float ChargeTime;
    public float AttackTime;
    public float RestTime;
    float CurPhaseTime { get; set; } = 0f;
    AttackPattern AttackStage { get; set; } = AttackPattern.Rest;
    Vector3 AttackTarget { get; set; }

    private void Start()
    {
        CurPhaseTime = Random.Range(0f, .15f);
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
                CurPhaseTime -= RestTime;
                AttackStage = AttackPattern.Charge;
                EnemySprite.sprite = CrabChargeUp;
            }
        }
        else if(AttackStage == AttackPattern.Charge)
        {
            if (CurPhaseTime > ChargeTime)
            {
                CurPhaseTime -= ChargeTime;
                AttackStage = AttackPattern.Attack;
                EnemySprite.sprite = CrabAttack;

                Vector3 actualTarget = Vector3.MoveTowards(transform.position, PlayerMobInstance.transform.position, MovementSpeed * AttackTime);
                float adjustedMagnitudeModifier = Mathf.Min(1.5f, Vector3.Distance(transform.position, actualTarget) * .3f);
                AttackTarget = actualTarget + new Vector3(Random.Range(-adjustedMagnitudeModifier, adjustedMagnitudeModifier), 0, Random.Range(-adjustedMagnitudeModifier, adjustedMagnitudeModifier));
            }
        }
        else if(AttackStage == AttackPattern.Attack)
        {
            Vector3 newTargetPosition = Vector3.MoveTowards(transform.position, AttackTarget, Time.deltaTime * MovementSpeed);
            Walk(newTargetPosition - transform.position);

            if (CurPhaseTime > AttackTime)
            {
                CurPhaseTime -= AttackTime;
                AttackStage = AttackPattern.Rest;
                EnemySprite.sprite = NeutralSprite;
            }
        }
    }
}
