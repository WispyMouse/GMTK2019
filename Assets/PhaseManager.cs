using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Movement, Explosion }
public class PhaseManager : MonoBehaviour
{
    public static GameState CurrentGameState { get; set; } = GameState.Movement;
    public GameObject ExplosionInstance;
    public PlayerMob PlayerMobInstance;
    public GameObject RuneCursorInstance;
    public LayerMask FloorMask;
    public LayerMask EnemyMask;

    Vector3 CameraOffsetFromPlayer { get; } = Vector3.up * 15f + Vector3.back * 5f;
    Vector3 CameraOffsetFromExplosionCursor { get; } = Vector3.up * 4f + Vector3.back * 20f;
    float TimeForExplosionCursorCameraApproach { get; } = 1.4f;
    float StandardCameraDownTilt { get; } = 70f;
    float ExplosionCameraDowntilt { get; } = 5f;
    float TimeForReturnToPlayerCameraApproach { get; } = .8f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && CurrentGameState == GameState.Movement && RuneCursorInstance.activeSelf)
        {
            StartExplosionPhase();
        }
        else
        {
            if (CurrentGameState == GameState.Movement)
            {
                HandleRuneCursor();
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
        }
    }

    void StartExplosionPhase()
    {
        CurrentGameState = GameState.Explosion;
        StartCoroutine(ExplosionCameraApproach());
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
        ExplosionInstance.gameObject.SetActive(true);
        yield return new WaitForSeconds(.6f);

        Collider[] enemyHits = Physics.OverlapSphere(RuneCursorInstance.transform.position, 3f, EnemyMask, QueryTriggerInteraction.Collide);

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

        CurrentGameState = GameState.Movement;
    }
}
