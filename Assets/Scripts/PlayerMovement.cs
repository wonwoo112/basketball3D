using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    public enum PlayerState {recieved, dribbled, held, free};
    public PlayerState state = PlayerState.free;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;

    private bool isDashing = false;
    private float dashTime;
    private float handDirection = 0f;  // 손방향 회전값 저장
    Rigidbody rb;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        moveAction = inputActions.Player.Move;
        jumpAction = inputActions.Player.Jump;
        dashAction = inputActions.Player.Dash;

        moveAction.Enable();
        jumpAction.Enable();
        dashAction.Enable();

        dashAction.started += ctx => StartDash();
        dashAction.canceled += ctx => EndDash();
        rb = GetComponent<Rigidbody>();
        dashTime = dashDuration;
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
        rb.velocity = new Vector3(moveInput.x * currentSpeed, rb.velocity.y, moveInput.y * currentSpeed);

        // 플레이어의 회전 적용
        transform.rotation = Quaternion.Euler(0, handDirection, 0);

        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                EndDash();
                dashTime = 0;
            }
        }
        else
        {
            dashTime += 0.5f * Time.deltaTime;
            if (dashTime >= dashDuration)
            {
                dashTime = dashDuration;
            }
        }
    }

    void Jump()
    {
        if (state != PlayerState.dribbled) {
            Debug.Log("Jump Pressed");
            if(transform.position.y <= 10.1) {
                rb.AddForce(Vector3.up * 35, ForceMode.Impulse);
            }  
        }
    }

    

    void StartDash()
    {
        Debug.Log("Dash Activated!");
        isDashing = true;
    }
    void EndDash()
    {
        Debug.Log("Dash Deactivated!");
        isDashing = false;
    }

    void UpdateHandDirection()
    {
        if (moveInput.sqrMagnitude > 0.01f)  // 움직이는 중일 때만 회전
        {
            handDirection = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
        }
    }
}
