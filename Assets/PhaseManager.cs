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
    public ParticleSystem PreExplosionParticleSystem;
    public ParticleSystem PostExplosionParticleSystem;
    public ParticleSystem PostExplosionYellowParticleSystem;
    public ParticleSystem LastWarmupExplosionParticleSystem;
    public PlayerMob PlayerMobInstance;
    public GameObject RuneCursorInstance;
    public MapGenerator MapGeneratorInstance;
    public LayerMask FloorMask;
    public LayerMask EnemyMask;
    public LevelManager LevelManagerInstance;
    public HealthManager HealthManagerInstance;
    public GameObject ExplosionScorchMarkInstance;

    public Image WaitCircle;
    public Transform ExhaustionTimeHud;

    Vector3 CameraOffsetFromPlayer { get; } = Vector3.up * 13f + Vector3.back * 4f;
    Vector3 CameraOffsetFromExplosionCursor { get; } = Vector3.up * 2.5f + Vector3.back * 5f;
    Vector3 CameraOffsetFromExhaustedPlayer { get; } = Vector3.up * 7.5f + Vector3.back * 2f;
    float TimeForExplosionCursorCameraApproach { get; } = 1.3f;
    float StandardCameraDownTilt { get; } = 75f;
    float ExplosionCameraDowntilt { get; } = 10f;
    float TimeForReturnToPlayerCameraApproach { get; } = .9f;
    bool FinishedPanningCamera { get; set; } = false;

    float ExplosionScale { get; set; } = 6f;
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
    public AudioClip VictorySound;

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
        if (PlayerPrefs.GetInt("BraggingRights", 0) == 1)
        {
            ExplosionScale = 12f;
        }
        else
        {
            ExplosionScale = 6f;
        }

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
        else if (CurrentGameState == GameState.Exhaustion && FinishedPanningCamera)
        {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, PlayerMobInstance.transform.position + CameraOffsetFromExhaustedPlayer, Time.deltaTime * 1f);
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
        if (MainMenuControl.SelectedLevel.FinalBossChapter)
        {
            return;
        }

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
        PlayerMobInstance.CastingState();
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
        SoundPlayer.PlaySound(VictorySound);
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

        PreExplosionParticleSystem.transform.position = RuneCursorInstance.transform.position;
        ParticleSystem.MainModule main = PreExplosionParticleSystem.main;
        main.startSpeed = -25f / 4f * ExplosionScale;
        ParticleSystem.ShapeModule shape = PreExplosionParticleSystem.shape;
        shape.radius = ExplosionScale;
        PreExplosionParticleSystem.gameObject.SetActive(true);
        PreExplosionParticleSystem.Play();

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
        LastWarmupExplosionParticleSystem.transform.position = RuneCursorInstance.transform.position;
        LastWarmupExplosionParticleSystem.gameObject.SetActive(true);
        LastWarmupExplosionParticleSystem.Play();

        yield return new WaitForSeconds(.8f);

        PreExplosionParticleSystem.gameObject.SetActive(false);
        LastWarmupExplosionParticleSystem.gameObject.SetActive(false);

        PostExplosionParticleSystem.transform.position = RuneCursorInstance.transform.position;
        PostExplosionParticleSystem.gameObject.SetActive(true);
        PostExplosionParticleSystem.Play();

        PostExplosionYellowParticleSystem.transform.position = RuneCursorInstance.transform.position;
        PostExplosionYellowParticleSystem.gameObject.SetActive(true);
        PostExplosionYellowParticleSystem.Play();

        ExplosionScorchMarkInstance.transform.position = RuneCursorInstance.transform.position;
        ExplosionScorchMarkInstance.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
        ExplosionScorchMarkInstance.transform.localScale = Vector3.one * ExplosionScale;
        ExplosionScorchMarkInstance.gameObject.SetActive(true);

        ExplosionInstance.transform.position = RuneCursorInstance.transform.position;
        ExplosionInstance.gameObject.SetActive(true);
        SoundPlayer.PlaySound(BigExplosionSound);

        float curExplosionTime = 0;
        float totalExplosionTime = .6f;

        while (curExplosionTime < totalExplosionTime)
        {
            ExplosionInstance.transform.localScale = Vector3.one * (curExplosionTime / totalExplosionTime) * ExplosionScale;
            curExplosionTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(.6f);

        Collider[] enemyHits = Physics.OverlapSphere(RuneCursorInstance.transform.position, ExplosionScale * .5f, EnemyMask, QueryTriggerInteraction.Collide);
        ExplosionHits = enemyHits.Length;

        for (int ii = 0; ii < enemyHits.Length; ii++)
        {
            EnemyMob hitEnemy = enemyHits[ii].gameObject.GetComponent<EnemyMob>();
            yield return hitEnemy.DefeatAnimationStartup(ExplosionInstance.transform.position);
        }

        yield return new WaitForSeconds(.4f);

        PostExplosionParticleSystem.gameObject.SetActive(false);
        PostExplosionYellowParticleSystem.gameObject.SetActive(false);

        ExplosionInstance.gameObject.SetActive(false);
        yield return ReturnCameraToPlayer();
    }

    IEnumerator ReturnCameraToPlayer()
    {
        float currentCameraTime = 0;
        Vector3 startingCameraPosition = Camera.main.transform.position;
        Vector3 targetPosition = PlayerMobInstance.transform.position + CameraOffsetFromExhaustedPlayer;

        while (currentCameraTime < TimeForReturnToPlayerCameraApproach / 2f)
        {
            currentCameraTime += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(startingCameraPosition, targetPosition, currentCameraTime / TimeForReturnToPlayerCameraApproach);
            Camera.main.transform.rotation = Quaternion.Euler(Mathf.Lerp(ExplosionCameraDowntilt, StandardCameraDownTilt, currentCameraTime / TimeForReturnToPlayerCameraApproach), 0, 0);
            yield return new WaitForEndOfFrame();
        }

        StartExhaustionPhase();

        while (currentCameraTime < TimeForReturnToPlayerCameraApproach)
        {
            currentCameraTime += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(startingCameraPosition, targetPosition, currentCameraTime / TimeForReturnToPlayerCameraApproach);
            Camera.main.transform.rotation = Quaternion.Euler(Mathf.Lerp(ExplosionCameraDowntilt, StandardCameraDownTilt, currentCameraTime / TimeForReturnToPlayerCameraApproach), 0, 0);
            yield return new WaitForEndOfFrame();
        }

        Camera.main.transform.position = targetPosition;
        Camera.main.transform.rotation = Quaternion.Euler(StandardCameraDownTilt, 0, 0);
        FinishedPanningCamera = true;
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
        SoundPlayer.PlaySound(MenuSelectSound);
        MainMenuControl.ShowLevelSelect = true;
        SceneManager.LoadScene(0);
    }

    public void PlayerPicksUpStaff()
    {
        PlayerHasStaff = true;
    }

    public void PlayerPicksUpGemOfBraggingRights()
    {
        PlayerPrefs.SetInt("BraggingRights", 1);

        CurrentGameState = GameState.Safe;
        WaitCircle.fillAmount = 0f;
        FlavorLabel.text = "We defeated the 「ENIGMA BEAST」!\nWe retrieved the 「GEM OF BRAGGING RIGHTS」!";
        AlwaysReturnToLevelSelect.gameObject.SetActive(false);

        StatisticsLabel.text = $"From now on, our 「EXPLOSION」 range is doubled!";

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
}
