using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;

public class ScreenOverlay : MonoBehaviour
{
    [SerializeField] private RectTransform overlayTransform;
    private float showingPos = 350;
    private float hiddenPos = -1100;

    [HideInInspector] public bool tweenCompleted;
    public bool tweenRunning;
    public void ShowOverlay()
    {
        tweenRunning = true;
        tweenCompleted = false;

        DoOverlayTween(showingPos);
    }

    public void HideOverlay()
    {
        tweenRunning = true;
        tweenCompleted = false;
        
        DoOverlayTween(hiddenPos);
    }

    private void DoOverlayTween(float targetPos)
    {
        System.Action<ITween<Vector3>> updateOverlayPosition = (t) =>
        {
            overlayTransform.anchoredPosition = t.CurrentValue;
        };

        System.Action<ITween<Vector3>> updateOverlayCompleted = (t) =>
        {
            tweenCompleted = true;
            tweenRunning = false;
            Debug.Log("Tween Completed");
        };

        Vector3 currentPos = overlayTransform.anchoredPosition;

        overlayTransform.gameObject.Tween("ShowOverlay", currentPos, new Vector3(currentPos.x, targetPos, currentPos.z), 1.5f, TweenScaleFunctions.CubicEaseInOut, updateOverlayPosition, updateOverlayCompleted);
    }
}
