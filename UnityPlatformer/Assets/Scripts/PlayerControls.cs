using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    public PlayerKeybinds playerControls;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    Vector2 moveDirection = Vector2.zero;
    private InputAction move;
    private Rigidbody2D rb;
    private Animator anim;
    private InputAction jump;
    private Transform transformPlayer;
    private bool isGrounded = false;
    private bool facingRight = true;
    public CameraFollowObject cameraFollowObject;
    private float fallSpeedThreshold;

    //Jump Mechanics
    private float elapsedTime;
    private bool jumpStarted;
    public float maxJumpTime = 5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        transformPlayer = GetComponent<Transform>();
        fallSpeedThreshold = CameraManager.instance.fallSpeedThreshold;
    }
    private void Awake()
    {
        playerControls = new();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();
        jump = playerControls.Player.Jump;
        jump.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
    }

    private void Update()
    {
        if (rb.velocity.y < fallSpeedThreshold && !CameraManager.instance.IsLerping && !CameraManager.instance.LerpedFromFall)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if (rb.velocity.y >= 0f && CameraManager.instance.IsLerping && CameraManager.instance.LerpedFromFall)
        {
            CameraManager.instance.LerpedFromFall = false;
            CameraManager.instance.LerpYDamping(false);
        }
        Movement();
    }
    private void FixedUpdate()
    {

    }

    private void Movement()
    {
        //Reads direction and sets velocity
        moveDirection = move.ReadValue<Vector2>();
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);

        //Sets the direction the characters facing
        /*
        if (rb.velocity != Vector2.zero)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
        */
        if (moveDirection.x > 0 && !facingRight)
        {
            transformPlayer.Rotate(0f, -180f, 0f);
            facingRight = true;

            cameraFollowObject.CallTurn();
        }
        else if (moveDirection.x < 0 && facingRight)
        {
            transformPlayer.Rotate(0f, 180f, 0f);
            facingRight = false;

            cameraFollowObject.CallTurn();
        }

        //Makes the character jump when space is pressed
        /* if (jump.WasPressedThisFrame() && rb.velocity.y == 0f)
        {
            jumpStarted = true;
        }
        if (jump.WasReleasedThisFrame() && rb.velocity.y == 0f)
        {
            jumpStarted = false;
        }
        if (jumpStarted)
        {
            if (elapsedTime < maxJumpTime)
            {
                elapsedTime += Time.deltaTime;
            }
        }
        if (!jumpStarted)
        {
            if (elapsedTime > 1f)
            {
                rb.AddForce(elapsedTime * jumpForce * Vector2.up, ForceMode2D.Impulse);
                elapsedTime = 0f;
            }
            else if (elapsedTime > 0f)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                elapsedTime = 0f;
            }
        }
            */
        if (jump.WasPressedThisFrame() && rb.velocity.y == 0f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}