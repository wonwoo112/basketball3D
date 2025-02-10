using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;


    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;

    private bool isDashing = false;
    private float dashTime;
    private float handDirection = 0f;  // 손방향 회전값 저장

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        moveAction = inputActions.Player.Move;
        jumpAction = inputActions.Player.Jump;
        dashAction = inputActions.Player.Dash;

        moveAction.Enable();
        jumpAction.Enable();
        dashAction.Enable();

        dashAction.performed += ctx => StartDash();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        dashAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        dashAction.Disable();
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        if (jumpAction.triggered)
        {
            Jump();
        }

        UpdateHandDirection();  // 손방향 업데이트
        MovePlayer();
    }

    void MovePlayer()
    {
        float currentSpeed = isDashing ? dashSpeed : baseSpeed;
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * currentSpeed * Time.deltaTime;
        transform.position += movement;

        // 플레이어의 회전 적용
        transform.rotation = Quaternion.Euler(0, handDirection, 0);

        if (isDashing && Time.time > dashTime)
        {
            isDashing = false;
        }
    }

    void Jump()
    {
        Debug.Log("Jump Pressed");
    }

    

    void StartDash()
    {
        Debug.Log("Dash Activated!");
        isDashing = true;
        dashTime = Time.time + dashDuration;
    }

    void UpdateHandDirection()
    {
        if (moveInput.sqrMagnitude > 0.01f)  // 움직이는 중일 때만 회전
        {
            handDirection = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
        }
    }
}
