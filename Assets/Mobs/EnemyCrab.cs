using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackPattern { Rest, Charge, Attack }
public class EnemyCrab : Mob
{
    public PlayerMob PlayerMobInstance { get; set; }
    const float MovementSpeed = 4.5f;

    float defeatAnimationRotationForce { get; set; }
    float defeatAnimationBounceHeight { get; set; }
    float defeatAnimationKnockback { get; set; }

    float timeForRotationStartup { get; } = .25f;
    float timeForDefeatAnimation { get; set; }

    FacesCamera FacesCameraComponent { get; set; }

    public bool EnemyActive { get; private set; } = true;
    public SpriteRenderer EnemySprite;

    public Sprite CrabNeutral;
    public Sprite CrabKO;
    public Sprite CrabChargeUp;
    public Sprite CrabAttack;

    float ChargeTime { get; } = .15f;
    float AttackTime { get; } = .4f;
    float RestTime { get; } = .5f;
    float CurPhaseTime { get; set; } = 0f;
    AttackPattern AttackStage { get; set; } = AttackPattern.Rest;
    Vector3 AttackTarget { get; set; }

    private void Awake()
    {
        FacesCameraComponent = GetComponentInChildren<FacesCamera>();

        // These creatures know, from the moment they are born, how much they will flip when they die
        // Fatalistic programming in action
        defeatAnimationRotationForce = Random.Range(10f, 270f);
        defeatAnimationBounceHeight = Random.Range(1f, 2.5f);
        defeatAnimationKnockback = Random.Range(1f, 2f);
        timeForDefeatAnimation = Random.Range(.7f, .9f);

        EnemySprite.sprite = CrabNeutral;
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
                EnemySprite.sprite = CrabChargeUp;
            }
        }
        else if(AttackStage == AttackPattern.Charge)
        {
            if (CurPhaseTime > ChargeTime)
            {
                CurPhaseTime = 0;
                AttackStage = AttackPattern.Attack;
                EnemySprite.sprite = CrabAttack;
                AttackTarget = PlayerMobInstance.transform.position;
            }
        }
        else if(AttackStage == AttackPattern.Attack)
        {
            Vector3 newTargetPosition = Vector3.MoveTowards(transform.position, AttackTarget, Time.deltaTime * MovementSpeed);
            Walk(newTargetPosition - transform.position);

            if (CurPhaseTime > AttackTime)
            {
                CurPhaseTime = 0;
                AttackStage = AttackPattern.Rest;
                EnemySprite.sprite = CrabNeutral;
            }
        }
    }

    public IEnumerator DefeatAnimationStartup(Vector3 explosionEpicenter)
    {
        EnemySprite.sprite = CrabKO;
        FacesCameraComponent.enabled = false;
        EnemyActive = false;
        StartCoroutine(DefeatAnimation(explosionEpicenter));
        yield return new WaitForSeconds(timeForRotationStartup);
    }

    IEnumerator DefeatAnimation(Vector3 explosionEpicenter)
    {
        float currentAnimationTime = 0f;
        float rotationDirection = transform.position.x < explosionEpicenter.x ? -1f : 1f;

        Vector3 startingPosition = transform.position;

        while (currentAnimationTime < timeForDefeatAnimation)
        {
            float currentTimeTick = currentAnimationTime / timeForDefeatAnimation;

            transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, 0, Mathf.Lerp(0, defeatAnimationRotationForce * rotationDirection, currentTimeTick));
            transform.position = new Vector3(startingPosition.x + Mathf.Lerp(0, defeatAnimationKnockback * rotationDirection, currentTimeTick), 
                Mathf.PingPong((currentTimeTick) * 2f * defeatAnimationBounceHeight, defeatAnimationBounceHeight), startingPosition.z);
            EnemySprite.color = new Color(1f, 1f, 1f, 1f - currentTimeTick);
            currentAnimationTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerMob playerMob = other.GetComponent<PlayerMob>();

        if (playerMob == null)
        {
            return;
        }

        HurtPlayer(playerMob);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMob playerMob = other.GetComponent<PlayerMob>();

        if (playerMob == null)
        {
            return;
        }

        HurtPlayer(playerMob);
    }

    void HurtPlayer(PlayerMob playerMobInstance)
    {
        if (PhaseManager.CurrentGameState != GameState.Movement && PhaseManager.CurrentGameState != GameState.Exhaustion)
        {
            return;
        }

        if (!EnemyActive)
        {
            return;
        }

        if (playerMobInstance.InHurtTime)
        {
            return;
        }

        playerMobInstance.TakeDamage(1);
    }
}
