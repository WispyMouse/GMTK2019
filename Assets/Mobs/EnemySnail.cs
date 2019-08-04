using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnail : EnemyMob
{
    public enum AttackPattern { Rest, Charge, Attack }
    public PlayerMob PlayerMobInstance { get; set; }

    public Sprite SnailChargeUp;
    public Sprite SnailFire;

    public float ChargeTime;
    public float AttackTime;
    public float RestTime;
    float CurPhaseTime { get; set; } = 0f;
    AttackPattern AttackStage { get; set; } = AttackPattern.Rest;
    Vector3 AttackTarget { get; set; }

    public AudioClip SnailAttackSound;
    public SnailFire SnailFirePF;

    private void Start()
    {
        CurPhaseTime = Random.Range(0f, .4f);
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
                EnemySprite.sprite = SnailChargeUp;
            }
        }
        else if (AttackStage == AttackPattern.Charge)
        {
            if (CurPhaseTime > ChargeTime)
            {
                CurPhaseTime -= ChargeTime;
                AttackStage = AttackPattern.Attack;
                EnemySprite.sprite = SnailFire;

                SoundPlayer.PlayPitchAdjustedSound(SnailAttackSound, .25f);
                SnailFire newFire = Instantiate(SnailFirePF, transform.position + Vector3.up * .5f, Quaternion.identity);
                newFire.Direction = (PlayerMobInstance.transform.position - transform.position).normalized;
            }
        }
        else if (AttackStage == AttackPattern.Attack)
        {
            if (CurPhaseTime > AttackTime)
            {
                CurPhaseTime -= AttackTime;
                AttackStage = AttackPattern.Rest;
                EnemySprite.sprite = NeutralSprite;
            }
        }
    }
}
