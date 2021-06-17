using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;

public class ScreenOverlay : MonoBehaviour
{
    private float showingAlpha = 1;
    private float hiddenAlpha = 0;

    [HideInInspector] public bool tweenCompleted;
    public bool tweenRunning;
    public void ShowOverlay()
    {
        tweenRunning = true;
        tweenCompleted = false;

        DoOverlayTween(showingAlpha);
    }

    public void HideOverlay()
    {
        tweenRunning = true;
        tweenCompleted = false;

        DoOverlayTween(hiddenAlpha);
    }

    private void DoOverlayTween(float targetAlpha)
    {
        System.Action<ITween<float>> updateOverlayPosition = (t) =>
        {
            GetComponent<CanvasGroup>().alpha = t.CurrentValue;
        };

        System.Action<ITween<float>> updateOverlayCompleted = (t) =>
        {
            tweenCompleted = true;
            tweenRunning = false;
            Debug.Log("Tween Completed");
        };

        float currentAlpha = GetComponent<CanvasGroup>().alpha;

        this.gameObject.Tween("ShowOverlay", currentAlpha, targetAlpha, 0.8f, TweenScaleFunctions.CubicEaseInOut, updateOverlayPosition, updateOverlayCompleted);
    }
}
