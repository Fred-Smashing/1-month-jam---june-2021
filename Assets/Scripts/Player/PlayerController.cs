using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PlayerController : MonoBehaviour
{
    private CompositeCollider2D _collider;
    private Rigidbody2D _body;
    private Animator _animator;

    [Header("Physics"), Space]
    [SerializeField] private Transform raycastPos;
    [SerializeField] private float raycastDistance = 0.05f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Controler"), Space]
    [SerializeField] private float speed = 3;
    [SerializeField] private float airSpeed = 3;
    [SerializeField] private float acceleration = 10;
    [SerializeField] private float airAcceleration = 5;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpRememberBuffer = 0.2f;
    [SerializeField] private float coyoteTime = 0.1f;

    private Vector3 defaultScale;

    private void Start()
    {
        _collider = GetComponent<CompositeCollider2D>();
        _body = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        defaultScale = transform.localScale;
    }

    private float horizontalInput;
    private bool jumpInput;
    private bool jumpRemember;
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        horizontalInput = Mathf.Clamp(horizontalInput, -1, 1);

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

        if (jumpRemember)
        {
            _body.AddForce(Vector2.up * calculateJumpForce(jumpHeight));
            StopCoroutine(CoyoteTimer());
            canJump = false;
            jumpRemember = false;
            isGrounded = false;
        }

        if (isGrounded)
        {
            canJump = true;
            velocity.x = Mathf.Lerp(velocity.x, speed * horizontalInput, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.Lerp(velocity.x, airSpeed * horizontalInput, airAcceleration * Time.deltaTime);
        }

        // Sticky Landing
        if (isGrounded && !wasGrounded && horizontalInput == 0)
        {
            velocity.y = 0;
            velocity.x = velocity.x / 2;
        }

        if (!isGrounded && wasGrounded)
        {
            StartCoroutine(CoyoteTimer());
        }

        velocity.y = _body.velocity.y;
        velocity.z = 0;

        _body.velocity = velocity;
        wasGrounded = isGrounded;
    }

    private void LateUpdate()
    {
        if (horizontalInput != 0)
        {
            _animator.SetBool("running", true);

            if (horizontalInput > 0)
            {
                transform.localScale = defaultScale;
            }
            else if (horizontalInput < 0)
            {
                transform.localScale = new Vector3(-defaultScale.x, defaultScale.y, defaultScale.z);
            }
        }
        else
        {
            _animator.SetBool("running", false);
        }

        _animator.SetBool("inAir", !isGrounded);
    }

    #region Physics Functions
    public float calculateJumpForce(float jumpHeight)
    {
        float force = Mathf.Sqrt((Physics.gravity.y * _body.gravityScale) * -2 * (jumpHeight * (1000 * _body.mass)));
        return force;
    }

    Vector3 hitpos;
    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(raycastPos.position, Vector2.down, raycastDistance, groundLayer);

        hitpos = hit.point;

        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }
    #endregion

    #region Coroutines
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
    #endregion

    #region Gizmos Drawing
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Ray(raycastPos.position, Vector2.down * raycastDistance));

        if (Application.isPlaying)
        {
            Gizmos.DrawWireSphere(hitpos, 0.1f);
        }
    }
    #endregion
}
