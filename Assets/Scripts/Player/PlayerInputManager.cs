using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private PlayerController _controller;

    [SerializeField] private float variationFrequency = 0.7f;
    [SerializeField] private bool randomizeInput = true;

    private float time;
    private void Start()
    {
        time = Time.realtimeSinceStartup;
    }

    private float horizontalInput;
    private bool jumpInput;
    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetKeyDown(KeyCode.Space);

        if (randomizeInput)
        {
            float sineTime = time - Time.realtimeSinceStartup;
            float variation = Mathf.Sin(-sineTime * variationFrequency);

            horizontalInput = horizontal * Mathf.Sign(variation);

            horizontalInput = Mathf.Clamp(horizontalInput, -1, 1);
        }
        else
        {
            horizontalInput = horizontal;
        }

        _controller.SetExternallyHorizontalInput(horizontalInput);
        _controller.SetExternallyJumpInput(jumpInput);
    }
}
