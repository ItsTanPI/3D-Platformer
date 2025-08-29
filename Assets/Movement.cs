using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform cameraTransform;
    [SerializeField] float _speed;
    [SerializeField] float _turnTime;
    [SerializeField] float _accleration;
    [SerializeField] bool Run;

    [Header("Jump")]
    [SerializeField] float _jumpHeight;
    [SerializeField] public float _gravityMultiplier = 1.0f;
    [SerializeField] float _gravity = -9.81f;


    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _groundCheck;
    [SerializeField] float _groundCheckHeight = 0.3f;
    float velocity;


    private InputMain _inputActions;
    private InputAction _moveAction;
    private InputAction _lookAction;


    private Vector2 moveInput;
    private Vector2 LookInput;

    private float turnSpeed;
    private float currentSpeed;

    public bool isRunning;
    public bool isAim;
    public bool isGrounded;
    public bool isFalling;


    private void Awake()
    {
        _inputActions = new InputMain();
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        GroundCheck();
        moveInput = _moveAction.ReadValue<Vector2>();
        LookInput = _lookAction.ReadValue<Vector2>();
        GravityandJump();
        Move();
    }

    void GroundCheck()
    {
        isGrounded = Physics.Raycast(_groundCheck.position, Vector3.down, _groundCheckHeight, _groundLayer);
    }


    void GravityandJump()
    {
        if (isGrounded && velocity < 0)
        {
            velocity = -5f;
            isFalling = false;

        }
        else if (!isGrounded && velocity < 0)
        {
            velocity += _gravity * Time.deltaTime * _gravityMultiplier;
            isFalling = true;
        }
        else if (!isGrounded && velocity > -5f)
        {
            velocity += _gravity * Time.deltaTime * _gravityMultiplier;
            isFalling = false;
        }

        if (isFalling && isGrounded)
        {
            isFalling = false;
        }

        if (isGrounded && _inputActions.Player.Jump.WasPerformedThisFrame() && !isAim && _gravityMultiplier != 0)
        {
            velocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity * _gravityMultiplier);
        }

        characterController.Move(Vector3.up * velocity * Time.deltaTime);
    }


    void Move()
    {
        if (moveInput.magnitude > 0f && !isAim)
        {

            currentSpeed = Mathf.Lerp(currentSpeed, _speed, Time.deltaTime * _accleration);
            float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSpeed, _turnTime);

            transform.rotation = Quaternion.Euler(0, smoothedAngle, 0);
            Vector3 moveDirection = Quaternion.Euler(0, smoothedAngle, 0) * Vector3.forward;


            characterController.Move(currentSpeed * Time.deltaTime * moveDirection.normalized);
        }
        else
        {
            if (currentSpeed < 0.1f)
            {
                currentSpeed = 0;
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 10);
            }

        }

    }


    private void OnEnable()
    {
        _inputActions.Player.Move.Enable();
        _moveAction = _inputActions.Player.Move;
        _inputActions.Player.Look.Enable();
        _lookAction = _inputActions.Player.Look;

        _inputActions.Player.Aim.Enable();
        _inputActions.Player.Jump.Enable();
        _inputActions.Player.Roll.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.Disable();
        _inputActions.Player.Aim.Disable();
        _inputActions.Player.Jump.Disable();
        _inputActions.Player.Look.Enable();
        _inputActions.Player.Roll.Disable();
    }

}
