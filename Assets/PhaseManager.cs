using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState { Movement, Explosion, Defeated, Exhaustion, Safe }
public class PhaseManager : MonoBehaviour
{
    public static GameState CurrentGameState { get; set; } = GameState.Movement;
    public GameObject ExplosionInstance;
    public PlayerMob PlayerMobInstance;
    public GameObject RuneCursorInstance;
    public MapGenerator MapGeneratorInstance;
    public LayerMask FloorMask;
    public LayerMask EnemyMask;
    public LevelManager LevelManagerInstance;
    public HealthManager HealthManagerInstance;

    public Image WaitCircle;
    public Transform ExhaustionTimeHud;

    Vector3 CameraOffsetFromPlayer { get; } = Vector3.up * 13f + Vector3.back * 4f;
    Vector3 CameraOffsetFromExplosionCursor { get; } = Vector3.up * 2.5f + Vector3.back * 5f;
    float TimeForExplosionCursorCameraApproach { get; } = .8f;
    float StandardCameraDownTilt { get; } = 75f;
    float ExplosionCameraDowntilt { get; } = 10f;
    float TimeForReturnToPlayerCameraApproach { get; } = .3f;

    float ExplosionScale { get; set; } = 5f;
    float CurExhaustionTime { get; set; } = 0;
    float ExhaustionTime { get; } = 4.5f;

    public Transform PostRoundHud;
    public Text FlavorLabel;
    public Text StatisticsLabel;
    public Button NextLevelButton;
    public Button AlwaysReturnToLevelSelect;
    public Text LevelNameLabel;

    int ExplosionHits { get; set; } = 0;
    bool PlayerHasStaff { get; set; } = false;
    public AudioClip BigExplosionSound;

    public AudioClip MenuSelectSound;

    public List<string> VictoryWords;
    public List<string> DefeatWords;

    private void Awake()
    {
        CurrentGameState = GameState.Movement;
        ExhaustionTimeHud.gameObject.SetActive(false);
        PostRoundHud.gameObject.SetActive(false);
        RuneCursorInstance.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (MainMenuControl.SelectedLevel == null)
        {
            MainMenuControl.SelectedLevel = LevelManagerInstance.GetLevel(0);
        }

        LevelNameLabel.text = MainMenuControl.SelectedLevel.LevelName;

        MapGeneratorInstance.GenerateMap(MainMenuControl.SelectedLevel);
    }

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
                case GameState.Exhaustion:
                    RuneCursorInstance.gameObject.SetActive(false);
                    HandleKOTime();
                    break;
                default:
                    RuneCursorInstance.gameObject.SetActive(false);
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToLevelSelect();
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
        if (!PlayerHasStaff)
        {
            RuneCursorInstance.gameObject.SetActive(false);
            return;
        }

        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;

        if (!Physics.Raycast(cursorRay, out floorHit, float.MaxValue, FloorMask))
        {
            RuneCursorInstance.SetActive(false);
        }
        else
        {
            RuneCursorInstance.SetActive(true);
            RuneCursorInstance.transform.position = new Vector3(floorHit.point.x, .1f, floorHit.point.z);
            RuneCursorInstance.transform.localScale = Vector3.one * ExplosionScale;
        }
    }

    void HandleKOTime()
    {
        ExhaustionTimeHud.gameObject.SetActive(true);
        CurExhaustionTime += Time.deltaTime;
        WaitCircle.fillAmount = 1f - (CurExhaustionTime / ExhaustionTime);

        if (CurExhaustionTime > ExhaustionTime)
        {
            StartSafePhase();
        }
    }

    void StartExplosionPhase()
    {
        CurrentGameState = GameState.Explosion;
        StartCoroutine(ExplosionCameraApproach());
    }

    void StartExhaustionPhase()
    {
        CurrentGameState = GameState.Exhaustion;
        PlayerMobInstance.ExhaustionState();
    }

    void StartDefeatedPhase()
    {
        CurrentGameState = GameState.Defeated;
        ExhaustionTimeHud.gameObject.SetActive(false);
        AlwaysReturnToLevelSelect.gameObject.SetActive(false);

        FlavorLabel.text = DefeatWords[Random.Range(0, DefeatWords.Count)].Replace("\\n", "\n");

        if (ExplosionHits > 0)
        {
            StatisticsLabel.text = $"(the {ExplosionHits} exploded things will respawn tomorrow)";
        }
        else
        {
            StatisticsLabel.text = $"";
        }
        
        PostRoundHud.gameObject.SetActive(true);
        NextLevelButton.gameObject.SetActive(false);
    }

    void StartSafePhase()
    {
        CurrentGameState = GameState.Safe;
        WaitCircle.fillAmount = 0f;
        FlavorLabel.text = VictoryWords[Random.Range(0, DefeatWords.Count)].Replace("\\n", "\n");
        AlwaysReturnToLevelSelect.gameObject.SetActive(false);

        if (ExplosionHits > 0)
        {
            StatisticsLabel.text = $"You got {ExplosionHits} of them that time!";
        }
        else
        {
            StatisticsLabel.text = $"";
        }

        PostRoundHud.gameObject.SetActive(true);

        LevelManagerInstance.ClearLevel(MainMenuControl.SelectedLevel, ExplosionHits);

        if (LevelManagerInstance.GameLevelCount > MainMenuControl.SelectedLevel.LevelIndex + 1)
        {
            NextLevelButton.gameObject.SetActive(true);
        }
        else
        {
            NextLevelButton.gameObject.SetActive(false);
        }
    }

    void UpdateHUD()
    {
        HealthManagerInstance.SetHealth(PlayerMobInstance.PlayerHealth);
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
        SoundPlayer.PlaySound(BigExplosionSound);
        yield return new WaitForSeconds(.6f);

        Collider[] enemyHits = Physics.OverlapSphere(RuneCursorInstance.transform.position, ExplosionScale * .5f, EnemyMask, QueryTriggerInteraction.Collide);
        ExplosionHits = enemyHits.Length;

        for (int ii = 0; ii < enemyHits.Length; ii++)
        {
            EnemyMob hitEnemy = enemyHits[ii].gameObject.GetComponent<EnemyMob>();
            yield return hitEnemy.DefeatAnimationStartup(ExplosionInstance.transform.position);
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

        StartExhaustionPhase();
    }

    public void PlayerIsDefeated()
    {
        StartDefeatedPhase();
    }

    public void LevelSelectButton()
    {
        SoundPlayer.PlaySound(MenuSelectSound);
        MainMenuControl.ShowLevelSelect = true;
        SceneManager.LoadScene(0);
    }

    public void RetryButton()
    {
        SoundPlayer.PlaySound(MenuSelectSound);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToNextLevelButton()
    {
        SoundPlayer.PlaySound(MenuSelectSound);
        MainMenuControl.SelectedLevel = LevelManagerInstance.GetLevel(MainMenuControl.SelectedLevel.LevelIndex + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToLevelSelect()
    {
        MainMenuControl.ShowLevelSelect = true;
        SceneManager.LoadScene(0);
    }

    public void PlayerPicksUpStaff()
    {
        PlayerHasStaff = true;
    }
}
