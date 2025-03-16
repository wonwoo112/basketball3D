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
    private Vector2 relPos;
    private enum Hand {left, right, na};
    private Hand holdingHand;
    private Hand dribblingHand;
    private float xSpeed;

    

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
        holdingHand = Hand.na;
        dribblingHand = Hand.na;
        xSpeed = 0f;
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
    float getNewRelX() {
        float oldX = relPos.x;
        if (holdingHand == dribblingHand) {
            return dribblingHand == Hand.left ? -1.0f : 1.0f;
        }
        else {
            return oldX + xSpeed * Time.deltaTime;
        }
        
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
        if (state == BallState.held || state == BallState.dribbled) {
            state = BallState.dribbled;
            relPos = new Vector2(-1, 0);
            if (holdingHand != Hand.right) {
                holdingHand = Hand.left;
            }
            dribblingHand = Hand.left;
        }
    }

    void DribbleRight() {
        if (state == BallState.held || state == BallState.dribbled) {
            state = BallState.dribbled;
            relPos = new Vector2(1, 0);
            if (holdingHand != Hand.left) {
                holdingHand = Hand.right;
            }
            dribblingHand = Hand.right;
        }
    }
}
