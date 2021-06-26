using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameManager instance => this;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject levelPrefab;

    [SerializeField] private TMPro.TMP_Text timerText;

    [SerializeField] private GameObject levelCompleteOverlay;
    [SerializeField] private GameObject levelFailedOverlay;

    private PlayerController currentPlayer = null;
    private LevelData currentLevel;
    private int currentLevelId = 0;

    private Transform spawnPoint;

    private ScreenOverlay overlay;

    [SerializeField] private bool cleanSaveData = false;

    [SerializeField] private List<GameObject> levelList = new List<GameObject>();

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    private void Start()
    {
        overlay = GameObject.FindGameObjectWithTag("Overlay").GetComponent<ScreenOverlay>();

        if (cleanSaveData)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
        else
        {
            if (CheckForSavedLevel())
            {
                currentLevelId = PlayerPrefs.GetInt("Current Level");
                levelPrefab = levelList[currentLevelId];
            }
            else
            {
                levelPrefab = levelList[0];
            }
        }

        StartLevel();
    }

    private void StartLevel()
    {
        SetupLevel();
        //SpawnPlayer();

        StartCoroutine(WaitForOverlayToStartGame());
        //StartTimer(currentLevel.levelTimeLimit);
    }

    private IEnumerator WaitForOverlayToStartGame()
    {
        overlay.HideOverlay();

        yield return new WaitWhile(() => !overlay.tweenCompleted);

        currentPlayer.SetControlLock(false);
        StartTimer(currentLevel.levelTimeLimit);
    }

    private void SetupLevel()
    {
        levelCompleteOverlay.SetActive(false);
        levelFailedOverlay.SetActive(false);

        GameObject level = Instantiate(levelPrefab);
        currentLevel = level.GetComponent<LevelData>();
        spawnPoint = currentLevel.spawnPoint;
        currentLevel.goalBottle.SetGameManager(instance);

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer.gameObject);
        }

        GameObject player = Instantiate(playerPrefab);

        player.transform.position = spawnPoint.position;

        currentPlayer = player.GetComponent<PlayerController>();

        currentPlayer.SetControlLock(true);
    }

    private bool timerRunning;
    private float timeRemaining;
    private void StartTimer(float time)
    {
        timeRemaining = time;
        timerRunning = true;
        levelEnded = false;
    }

    private void Update()
    {
        if (timerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;

                timerText.text = string.Format("Sobriety in " + "({0:0.0})", timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;

                KillPlayer();
            }
        }
    }

    private bool levelEnded = false;
    public void CompletedLevel()
    {
        if (!levelEnded)
        {
            currentPlayer.SetControlLock(true);

            timerRunning = false;

            levelCompleteOverlay.SetActive(true);

            SaveLevelById();

            var button = GameObject.Find("Next Level Button");

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(button);

            _audioSource.clip = winSound;
            _audioSource.Play();

            levelEnded = true;
        }
    }

    public void FailedLevel()
    {
        if (!levelEnded)
        {
            currentPlayer.SetControlLock(true);

            levelFailedOverlay.SetActive(true);

            var button = GameObject.Find("Retry Level Button");

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(button);

            _audioSource.clip = loseSound;
            _audioSource.Play();

            levelEnded = true;
        }
    }

    public void KillPlayer()
    {
        currentPlayer.Kill();

        timerRunning = false;

        FailedLevel();
    }

    #region Level Loading
    public void NextLevel()
    {
        overlay.ShowOverlay();

        StartCoroutine(LoadNextLevelCoroutine());

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator LoadNextLevelCoroutine()
    {
        yield return new WaitWhile(() => !overlay.tweenCompleted);

        Destroy(currentLevel.gameObject);
        Destroy(currentPlayer.gameObject);

        currentLevelId++;

        if (currentLevelId > levelList.Count - 1)
        {
            // Reached the end of the levels
            // Reset Save Data and Return to main menu
            ResetSavedLevel();
            QuitGame();
        }
        else
        {
            levelPrefab = levelList[currentLevelId];

            SetupLevel();

            StartCoroutine(WaitForOverlayToStartGame());
        }

        //if (currentLevel.nextLevel != null)
        //{
        //    levelPrefab = currentLevel.nextLevel;

        //    SetupLevel();

        //    StartCoroutine(WaitForOverlayToStartGame());
        //}
        //else
        //{
        //    QuitGame();
        //}
    }

    public void RestartLevel()
    {
        overlay.ShowOverlay();
        StartCoroutine(RestartLevelCoroutine());
    }

    private IEnumerator RestartLevelCoroutine()
    {
        yield return new WaitWhile(() => !overlay.tweenCompleted);

        Destroy(currentLevel.gameObject);
        Destroy(currentPlayer.gameObject);

        SetupLevel();

        StartCoroutine(WaitForOverlayToStartGame());
    }
    #endregion

    #region Save Data Handling
    private bool CheckForSavedLevel()
    {
        return PlayerPrefs.HasKey("Current Level");
    }

    private void SaveLevelById()
    {
        if (currentLevel.nextLevel != null)
        {
            PlayerPrefs.SetInt("Current Level", currentLevelId);
            PlayerPrefs.Save();
        }
    }

    private void ResetSavedLevel()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
    #endregion

    #region Quit Game
    public void QuitGame()
    {
        PlayerPrefs.Save();
        overlay.ShowOverlay();
        StartCoroutine(QuitGameCoroutine());
    }

    private IEnumerator QuitGameCoroutine()
    {
        yield return new WaitWhile(() => !overlay.tweenCompleted);

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MenuScene");
    }
    #endregion
}
