using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private Collider2D _collider;
    private Rigidbody2D _body;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.05f;
    [SerializeField] private LayerMask groundLayer;

    private float speed = 6;
    private float acceleration = 10;
    private float jumpHeight = 3f;
    private float jumpRememberBuffer = 0.2f;
    private float coyoteTime = 0.1f;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _body = GetComponent<Rigidbody2D>();
    }

    public float horizontalInput;
    public bool jumpInput;
    public bool jumpRemember;
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        jumpInput = Input.GetKeyDown(KeyCode.Space);

        if (jumpInput && canJump)
        {
            jumpRemember = true;
            StartCoroutine(JumpRememberTimer());
        }
    }

    [SerializeField] private Vector3 velocity = Vector3.zero;
    [SerializeField] private bool canJump;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool wasGrounded;
    private void FixedUpdate()
    {
        isGrounded = IsGrounded();

        if (isGrounded)
        {
            canJump = true;
        }

        if (jumpRemember)
        {
            _body.AddForce(Vector2.up * calculateJumpForce(jumpHeight));
            StopCoroutine(CoyoteTimer());
            canJump = false;
            jumpRemember = false;
        }

        if (!isGrounded && wasGrounded)
        {
            StartCoroutine(CoyoteTimer());
        }

        velocity.x = Mathf.Lerp(velocity.x, speed * horizontalInput, acceleration * Time.deltaTime);
        velocity.y = _body.velocity.y;
        velocity.z = 0;

        _body.velocity = velocity;

        wasGrounded = isGrounded;
    }

    public float calculateJumpForce(float jumpHeight)
    {
        float force = Mathf.Sqrt((Physics.gravity.y * _body.gravityScale) * -2f * (jumpHeight * 1000));
        return force;
    }

    private bool IsGrounded()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(groundCheck.position, groundRadius, groundLayer);

        foreach (Collider2D collision in collisions)
        {
            if (collision.CompareTag("Platforms"))
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator JumpRememberTimer()
    {
        yield return new WaitForSeconds(jumpRememberBuffer);

        jumpRemember = false;
    }

    private IEnumerator CoyoteTimer()
    {
        yield return new WaitForSeconds(coyoteTime);

        canJump = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
