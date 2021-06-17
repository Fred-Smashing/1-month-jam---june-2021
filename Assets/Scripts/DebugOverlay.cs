using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOverlay : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text sceneText;
    [SerializeField] private TMPro.TMP_Text fpsText;
    [SerializeField] private TMPro.TMP_Text updateText;

    private UnityEngine.SceneManagement.Scene currentScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene();
    private int framesPerSeccond;
    private float renderUpdateMs;
    private float fixedUpdateMs;

    private float startTime;
    private float fixedStartTime;

    private void Awake()
    {
        StartCoroutine(DebugUiUpdate());
    }

    private void Update()
    {
        startTime = Time.realtimeSinceStartup;
        StartCoroutine(UpdateWait());
    }

    private void FixedUpdate()
    {
        fixedStartTime = Time.realtimeSinceStartup;
        StartCoroutine(FixedUpdateWait());
    }

    private IEnumerator UpdateWait()
    {
        yield return new WaitForEndOfFrame();
        renderUpdateMs = (Time.realtimeSinceStartup - startTime) * 1000;
    }

    private IEnumerator FixedUpdateWait()
    {
        yield return new WaitForFixedUpdate();
        fixedUpdateMs = (Time.realtimeSinceStartup - fixedStartTime) * 1000;
    }

    IEnumerator DebugUiUpdate()
    {
        yield return new WaitForSecondsRealtime(1f);

        framesPerSeccond = (int)(1.0f / Time.smoothDeltaTime);

        sceneText.text = string.Format("Scene | index: {0} name: {1}", currentScene.buildIndex, currentScene.name);
        fpsText.text = string.Format("FPS: {0}", framesPerSeccond);
        updateText.text = string.Format("rUpdate:{0:0.00}ms fUpdate:{1:0.00}ms", renderUpdateMs, fixedUpdateMs);

        StartCoroutine(DebugUiUpdate());
    }
}

