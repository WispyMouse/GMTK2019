using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCrab : MonoBehaviour
{
    public PlayerMob PlayerMobInstance { get; set; }
    const float MovementSpeed = 3.5f;

    float defeatAnimationRotationForce { get; set; }
    float defeatAnimationBounceHeight { get; set; }
    float defeatAnimationKnockback { get; set; }

    float timeForRotationStartup { get; } = .25f;
    float timeForDefeatAnimation { get; set; }

    FacesCamera FacesCameraComponent { get; set; }

    public bool EnemyActive { get; private set; } = true;
    public SpriteRenderer EnemySprite;

    private void Awake()
    {
        FacesCameraComponent = GetComponentInChildren<FacesCamera>();

        // These creatures know, from the moment they are born, how much they will flip when they die
        // Fatalistic programming in action
        defeatAnimationRotationForce = Random.Range(10f, 270f);
        defeatAnimationBounceHeight = Random.Range(1f, 2.5f);
        defeatAnimationKnockback = Random.Range(1f, 2f);
        timeForDefeatAnimation = Random.Range(.7f, .9f);
    }

    private void Update()
    {
        if (PhaseManager.CurrentGameState == GameState.Movement)
        {
            HandleMovement();
            return;
        }        
    }

    void HandleMovement()
    {
        Vector3 targetPosition = Vector3.MoveTowards(transform.position, PlayerMobInstance.transform.position, Time.deltaTime * MovementSpeed);
        transform.position = targetPosition;
    }

    public IEnumerator DefeatAnimationStartup(Vector3 explosionEpicenter)
    {
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
        if (!EnemyActive)
        {
            return;
        }

        if (playerMobInstance.InHurtTime)
        {
            return;
        }

        playerMobInstance.TakeDamage(20f);
    }
}
