using UnityEngine;
using UnityEngine.InputSystem;

public class BallMove : MonoBehaviour
{
    private Rigidbody rb; 
    private Collider col;
    private Transform holdPoint;
    private enum BallState {recieved, dribbled, held, free};
    private BallState state = BallState.free;
    private PlayerInputActions inputActions;
    private InputAction dribbleLeftAction;
    private InputAction dribbleRightAction;
    private InputAction shootAction;
    private InputAction passAction;
    private InputAction holdAction;
    [SerializeField] private float cooltime;
    // 쿨타임과 함께 누가 공을 마지막에 잡았는지도 저장해야 함
    private GameObject player;
    private BallDribble ballDribble;
    private bool leftPressed;
    private bool rightPressed;
    private Vector2 relPos;

    void Start()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        dribbleLeftAction = inputActions.Player.DribbleLeft;
        dribbleRightAction = inputActions.Player.DribbleRight;
        shootAction = inputActions.Player.Shoot;
        passAction = inputActions.Player.Pass;
        holdAction = inputActions.Player.Hold;
        dribbleLeftAction.Enable();
        dribbleRightAction.Enable();
        shootAction.Enable();
        passAction.Enable();
        holdAction.Enable();
        player = null;
        ballDribble = new BallDribble();
        relPos = new Vector2(0.0f, 0.0f);
    }
    
    bool isPossessed() {
        return state != BallState.free;
    }
    

    void OnCollisionEnter(Collision collision)
    {
        if (!isPossessed() && collision.gameObject.CompareTag("Player") && cooltime <= 0)
        {
            // 플레이어의 BallHoldPoint 찾기
            holdPoint = collision.transform.Find("BallHoldPoint");
            if (holdPoint != null)
            {
                HoldBall();
                player = collision.gameObject;
            }
        }
    }
    Vector3 To3D(Vector2 v) {
        Vector3 leftHand = player.transform.Find("LeftHand").position;
        Vector3 rightHand = player.transform.Find("RightHand").position;
        Vector3 lerp = Vector3.Lerp(leftHand, rightHand, (v.x + 1) / 2);
        return new Vector3(lerp.x, v.y, lerp.z);
    }
    void HoldBall()
    {
        state = BallState.recieved;
        rb.isKinematic = true; // 물리 효과 제거
        rb.velocity = Vector3.zero;
        transform.position = holdPoint.position;
        transform.parent = holdPoint; // 공을 플레이어 손에 고정
        col.isTrigger = true; // 충돌 무시
    }
    void ReleaseBall() {
        state = BallState.free;
        rb.isKinematic = false;
        transform.parent = null;
        col.isTrigger = false;
        cooltime = 1.0f;
    }
    void Update()
    {
        leftPressed = false;
        rightPressed = false;
        if (cooltime > 0)
        {
            cooltime -= Time.deltaTime;
        }
        if (shootAction.triggered)
        {
            Shoot();
        }
        else if (passAction.triggered)
        {
            Pass();
        }
        else if (holdAction.triggered)
        {
            Hold();
        }
        else if (dribbleLeftAction.triggered)
        {
            DribbleLeft();
        }
        else if (dribbleRightAction.triggered)
        {
            DribbleRight();
        }
        if (state == BallState.dribbled) {
            relPos = ballDribble.Update(leftPressed, rightPressed, relPos, Time.deltaTime);
            transform.position = To3D(relPos);
            if (ballDribble.isDribblingGetter() == false) {
                ReleaseBall();
            }
        }
    }
    void Shoot() {
        if (isPossessed()) {
            ReleaseBall();
        }
    }
    void Pass() {
        if (isPossessed()) {
            ReleaseBall();
        }
    }
    void Hold() {
        if (state == BallState.dribbled) {
            state = BallState.held;
        }
    }
    void DribbleLeft() {
        if(state == BallState.recieved || state == BallState.dribbled) {
            ballDribble.isDribblingSetter(true);
            leftPressed = true;
            state = BallState.dribbled;
            if (state == BallState.recieved) {
                relPos = new Vector2(-1.0f, 0.0f);
                ballDribble.holdingHand = BallDribble.Direction.left;
            }
        }
    }

    void DribbleRight() {
        if(state == BallState.recieved || state == BallState.dribbled) {
            ballDribble.isDribblingSetter(true);
            rightPressed = true;
            state = BallState.dribbled;
            if (state == BallState.recieved) {
                relPos = new Vector2(1.0f, 0.0f);
                ballDribble.holdingHand = BallDribble.Direction.right;
            }
        }
    }
}
