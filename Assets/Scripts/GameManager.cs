using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameManager instance => this;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject levelPrefab;

    [SerializeField] private TMPro.TMP_Text timerText;

    private PlayerController currentPlayer = null;
    private LevelData currentLevel;

    private Transform spawnPoint;

    private ScreenOverlay overlay;

    private void Start()
    {
        overlay = GameObject.FindGameObjectWithTag("Overlay").GetComponent<ScreenOverlay>();

        StartLevel();
    }

    private void StartLevel()
    {
        SetupLevel();
        //SpawnPlayer();

        StartCoroutine(WaitForOverlay());
        //StartTimer(currentLevel.levelTimeLimit);
    }

    private IEnumerator WaitForOverlay()
    {
        overlay.HideOverlay();

        yield return new WaitWhile(() => !overlay.tweenCompleted);

        SpawnPlayer();
        StartTimer(currentLevel.levelTimeLimit);
    }

    private void SetupLevel()
    {
        GameObject level = Instantiate(levelPrefab);
        currentLevel = level.GetComponent<LevelData>();
        spawnPoint = currentLevel.spawnPoint;
        currentLevel.goalBottle.SetGameManager(instance);
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

        currentPlayer.SetControlLock(false);
    }

    private bool timerRunning;
    private float timeRemaining;
    private void StartTimer(float time)
    {
        timeRemaining = time;
        timerRunning = true;
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
            }
        }
    }

    public void CompletedLevel()
    {
        //TODO: implement level switching
        Debug.Log("WIN");
    }

    public void RestartLevel()
    {
        //TODO: implement level restart
    }
}
