using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { Movement, Explosion, Defeated, Exhaustion }
public class PhaseManager : MonoBehaviour
{
    public static GameState CurrentGameState { get; set; } = GameState.Movement;
    public GameObject ExplosionInstance;
    public PlayerMob PlayerMobInstance;
    public GameObject RuneCursorInstance;
    public LayerMask FloorMask;
    public LayerMask EnemyMask;

    Vector3 CameraOffsetFromPlayer { get; } = Vector3.up * 10f + Vector3.back * 2.5f;
    Vector3 CameraOffsetFromExplosionCursor { get; } = Vector3.up * 2f + Vector3.back * 10f;
    float TimeForExplosionCursorCameraApproach { get; } = .7f;
    float StandardCameraDownTilt { get; } = 70f;
    float ExplosionCameraDowntilt { get; } = 5f;
    float TimeForReturnToPlayerCameraApproach { get; } = .5f;

    float ExplosionScale { get; set; } = 4f;

    public Slider HealthSlider;

    private void Update()
    {
        UpdateHUD();

        if (Input.GetMouseButtonDown(1) && CurrentGameState == GameState.Movement && RuneCursorInstance.activeSelf)
        {
            StartExplosionPhase();
        }
        else
        {
            switch (CurrentGameState)
            {
                case GameState.Movement:
                    HandleRuneCursor();
                    break;
                default:
                    RuneCursorInstance.gameObject.SetActive(false);
                    break;
            }
        }
    }

    private void LateUpdate()
    {
        HandleCamera();
    }

    void HandleCamera()
    {
        if (CurrentGameState == GameState.Movement)
        {
            Camera.main.transform.position = PlayerMobInstance.transform.position + CameraOffsetFromPlayer;
        }
    }

    void HandleRuneCursor()
    {
        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;

        if (!Physics.Raycast(cursorRay, out floorHit, float.MaxValue, FloorMask))
        {
            RuneCursorInstance.SetActive(false);
        }
        else
        {
            RuneCursorInstance.SetActive(true);
            RuneCursorInstance.transform.position = floorHit.point + Vector3.up * .1f;
            RuneCursorInstance.transform.localScale = Vector3.one * ExplosionScale;
        }
    }

    void StartExplosionPhase()
    {
        CurrentGameState = GameState.Explosion;
        StartCoroutine(ExplosionCameraApproach());
    }

    void UpdateHUD()
    {
        HealthSlider.maxValue = PlayerMobInstance.PlayerMaxHealth;
        HealthSlider.value = PlayerMobInstance.PlayerHealth;
    }

    IEnumerator ExplosionCameraApproach()
    {
        float currentCameraTime = 0;
        Vector3 startingCameraPosition = Camera.main.transform.position;
        Vector3 targetPosition = RuneCursorInstance.transform.position + CameraOffsetFromExplosionCursor;

        while (currentCameraTime < TimeForExplosionCursorCameraApproach)
        {
            currentCameraTime += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(startingCameraPosition, targetPosition, currentCameraTime / TimeForExplosionCursorCameraApproach);
            Camera.main.transform.rotation = Quaternion.Euler(Mathf.Lerp(StandardCameraDownTilt, ExplosionCameraDowntilt, currentCameraTime / TimeForExplosionCursorCameraApproach), 0, 0);
            yield return new WaitForEndOfFrame();
        }

        Camera.main.transform.position = targetPosition;
        Camera.main.transform.rotation = Quaternion.Euler(ExplosionCameraDowntilt, 0, 0);

        yield return ShowExplosion();
    }

    IEnumerator ShowExplosion()
    {
        ExplosionInstance.transform.position = RuneCursorInstance.transform.position;
        ExplosionInstance.transform.localScale = Vector3.one * ExplosionScale;
        ExplosionInstance.gameObject.SetActive(true);
        yield return new WaitForSeconds(.6f);

        Collider[] enemyHits = Physics.OverlapSphere(RuneCursorInstance.transform.position, ExplosionScale, EnemyMask, QueryTriggerInteraction.Collide);

        for (int ii = 0; ii < enemyHits.Length; ii++)
        {
            EnemyCrab hitCrab = enemyHits[ii].gameObject.GetComponent<EnemyCrab>();
            yield return hitCrab.DefeatAnimationStartup(ExplosionInstance.transform.position);
        }

        yield return new WaitForSeconds(.4f);

        ExplosionInstance.gameObject.SetActive(false);
        yield return ReturnCameraToPlayer();
    }

    IEnumerator ReturnCameraToPlayer()
    {
        float currentCameraTime = 0;
        Vector3 startingCameraPosition = Camera.main.transform.position;
        Vector3 targetPosition = PlayerMobInstance.transform.position + CameraOffsetFromPlayer;

        while (currentCameraTime < TimeForReturnToPlayerCameraApproach)
        {
            currentCameraTime += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(startingCameraPosition, targetPosition, currentCameraTime / TimeForReturnToPlayerCameraApproach);
            Camera.main.transform.rotation = Quaternion.Euler(Mathf.Lerp(ExplosionCameraDowntilt, StandardCameraDownTilt, currentCameraTime / TimeForReturnToPlayerCameraApproach), 0, 0);
            yield return new WaitForEndOfFrame();
        }

        Camera.main.transform.position = targetPosition;
        Camera.main.transform.rotation = Quaternion.Euler(StandardCameraDownTilt, 0, 0);

        CurrentGameState = GameState.Exhaustion;
        PlayerMobInstance.ExhaustionState();
    }

    public void PlayerIsDefeated()
    {
        CurrentGameState = GameState.Defeated;
    }
}
