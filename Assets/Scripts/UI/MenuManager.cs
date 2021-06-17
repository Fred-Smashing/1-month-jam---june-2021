using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private ScreenOverlay overlay;
    [SerializeField] private GameObject controlPanel;

    private bool controlsVisible = false;

    private void Start()
    {
        controlPanel.SetActive(false);

        overlay = GameObject.FindGameObjectWithTag("Overlay").GetComponent<ScreenOverlay>();
        overlay.HideOverlay();

        SetObjectSelected(GameObject.Find("Start Button").gameObject);
    }

    public void OnStartGamePressed()
    {
        StartCoroutine(StartGameOnOverlayShow());
    }

    private IEnumerator StartGameOnOverlayShow()
    {
        overlay.ShowOverlay();

        yield return new WaitWhile(() => !overlay.tweenCompleted);

        SceneManager.LoadSceneAsync("GameScene");
    }

    public void OnControlsPressed()
    {
        controlPanel.SetActive(true);
        SetObjectSelected();
        StartCoroutine(AllowClosingControlOverlay());
    }

    private IEnumerator AllowClosingControlOverlay()
    {
        yield return new WaitForSeconds(0.2f);
        controlsVisible = true;
    }

    private void Update()
    {
        if (controlsVisible)
        {
            if (Input.anyKey)
            {
                controlPanel.SetActive(false);
                controlsVisible = false;

                SetObjectSelected(GameObject.Find("Controls Button").gameObject);
            }
        }
    }

    private void SetObjectSelected(GameObject selectedObject = null)
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(selectedObject);
    }
}
