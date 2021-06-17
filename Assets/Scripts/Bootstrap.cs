using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameMode gameMode = GameMode.STANDARD;

    private enum GameMode
    {
        STANDARD,
        DEBUG
    }

    private void Start()
    {
        switch (gameMode)
        {
            case GameMode.STANDARD:
                SetDebugOverlayState(false);
                StartGame();
                break;

            case GameMode.DEBUG:
                SetDebugOverlayState(true);
                StartGame();
                break;
        }

    }

    private void SetDebugOverlayState(bool enabled)
    {
        var debugOverlay = GameObject.Find("Debug Overlay");

        debugOverlay.SetActive(enabled);

        if (enabled)
        {
            debugOverlay.AddComponent<DoNotDestroyOnLoad>();
        }
    }

    private void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MenuScene");
    }
}
