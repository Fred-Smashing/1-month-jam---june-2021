using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private ScreenOverlay overlay;

    private void Start()
    {
        overlay = GameObject.FindGameObjectWithTag("Overlay").GetComponent<ScreenOverlay>();
        overlay.HideOverlay();
    }

    public void OnStartGamePressed()
    {
        StartCoroutine(StartGameOnOverlayShow());
    }

    private void ShowControls()
    {

    }

    private IEnumerator StartGameOnOverlayShow()
    {
        overlay.ShowOverlay();

        yield return new WaitWhile(() => !overlay.tweenCompleted);

        SceneManager.LoadSceneAsync("GameScene");
    }
}
