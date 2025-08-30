using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    float velocity;

    [Header("Camera")]
    [SerializeField] bool isFirstPerson = false;
    [SerializeField] CinemachineVirtualCamera _fpCamera;
    [SerializeField] CinemachineFreeLook _tpCamera;



    [Header("Animation")]
    [SerializeField] Animator _animator;



    private InputMain _inputActions;
    private InputAction _moveAction;
    private InputAction _lookAction;


    private Vector2 moveInput;
    private Vector2 LookInput;

    private float turnSpeed;
    private float currentSpeed;

    [Header("State")]
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
        if (_inputActions.Player.Roll.WasPerformedThisFrame())
        {
            ChangeCamera();
        }

        if (isFirstPerson)
        {
            float yaw = cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, yaw, 0);
        }

        GroundCheck();
        moveInput = _moveAction.ReadValue<Vector2>();
        LookInput = _lookAction.ReadValue<Vector2>();
        GravityandJump();
        Move();


        _animator.SetFloat("Speed", currentSpeed / _speed);
    }

    void ChangeCamera()
    {
        isFirstPerson = !isFirstPerson;

        if(isFirstPerson)
        {
            _fpCamera.enabled = true;
            _tpCamera.enabled = false;
            _fpCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = _tpCamera.m_XAxis.Value;

        }
        else
        {
            _fpCamera.enabled = false;
            _tpCamera.enabled = true;
            _tpCamera.m_XAxis.Value = 0.0f;
        }

    }

    void GroundCheck()
    {
        isGrounded = Physics.Raycast(_groundCheck.position, Vector3.down, _groundCheckHeight, _groundLayer);
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
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

        if (coyoteTimeCounter > 0 && _inputActions.Player.Jump.WasPerformedThisFrame() && !isAim && _gravityMultiplier != 0)
        {
            _animator.SetTrigger("Jump");
            velocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity * _gravityMultiplier);
            coyoteTimeCounter = 0;
        }

        characterController.Move(Time.deltaTime * velocity * Vector3.up);
    }


    void Move()
    {
        if (moveInput.magnitude > 0f && !isAim)
        {

            currentSpeed = Mathf.Lerp(currentSpeed, _speed, Time.deltaTime * _accleration);
            if (isFirstPerson) 
            {

               

                Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
                characterController.Move(currentSpeed * Time.deltaTime * moveDirection.normalized);
            }
            else 
            {
                float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSpeed, _turnTime);

                transform.rotation = Quaternion.Euler(0, smoothedAngle, 0);
                Vector3 moveDirection = Quaternion.Euler(0, smoothedAngle, 0) * Vector3.forward;

                characterController.Move(currentSpeed * Time.deltaTime * moveDirection.normalized);
            }
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
