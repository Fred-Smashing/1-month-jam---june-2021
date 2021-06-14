using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _body;
    private Collider2D _collider;
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

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
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
    [HideInInspector] public Vector2 velocityLastFrame;
    private void FixedUpdate()
    {
        isGrounded = IsGrounded();

        if (jumpRemember)
        {
            Jump(CalculateJumpForce(jumpHeight));
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

        StickyLanding();

        if (!isGrounded && wasGrounded)
        {
            StartCoroutine(CoyoteTimer());
        }

        velocity.y = _body.velocity.y;
        velocity.z = 0;
        _body.velocity = velocity;

        wasGrounded = isGrounded;
        velocityLastFrame = _body.velocity;
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
    private void Jump(float jumpForce)
    {
        _body.AddForce(Vector2.up * jumpForce);
        StopCoroutine(CoyoteTimer());
        canJump = false;
        jumpRemember = false;
        isGrounded = false;
    }

    public float CalculateJumpForce(float jumpHeight)
    {
        float force = Mathf.Sqrt((Physics.gravity.y * _body.gravityScale) * -2 * (jumpHeight * (1000 * _body.mass)));
        return force;
    }

    private void StickyLanding()
    {
        if (isGrounded && !wasGrounded && horizontalInput == 0)
        {
            velocity.y = 0;
            velocity.x = velocity.x / 2;
        }
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
