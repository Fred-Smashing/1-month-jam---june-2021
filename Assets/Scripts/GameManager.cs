using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject levelPrefab;

    [SerializeField] private TMPro.TMP_Text timerText;

    private PlayerController currentPlayer = null;
    private LevelData currentLevel;

    private Transform spawnPoint;

    private void Start()
    {
        StartLevel();
    }

    private void StartLevel()
    {
        SetupLevel();
        SpawnPlayer();
        StartTimer(currentLevel.levelTimeLimit);
    }

    private void SetupLevel()
    {
        GameObject level = Instantiate(levelPrefab);
        currentLevel = level.GetComponent<LevelData>();
        spawnPoint = currentLevel.spawnPoint;
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
}
