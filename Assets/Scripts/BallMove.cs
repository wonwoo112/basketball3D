using UnityEngine;
using UnityEngine.InputSystem;

public class BallMove : MonoBehaviour
{
    private Rigidbody rb; 
    private Collider col;
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
    [SerializeField] private Vector2 relPos;

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
            player = collision.gameObject;
            HoldBall(false);
        }
    }
    Vector3 To3D(Vector2 v) {
        Vector3 leftHand = player.transform.Find("LeftHand").position;
        Vector3 rightHand = player.transform.Find("RightHand").position;
        Vector3 lerp = Vector3.Lerp(leftHand, rightHand, (v.x + 1) / 2);
        float y = player.transform.position.y + v.y * player.transform.localScale.y;
        return new Vector3(lerp.x, y, lerp.z);
    }
    void HoldBall(bool whileDribbling)
    {
        if (whileDribbling) {
            state = BallState.held;
            player.GetComponent<PlayerMovement>().state = PlayerMovement.PlayerState.held;
        }
        else {
            state = BallState.recieved;
            player.GetComponent<PlayerMovement>().state = PlayerMovement.PlayerState.recieved;
        }
        rb.isKinematic = true; // 물리 효과 제거
        rb.velocity = Vector3.zero;
        transform.parent = player.transform;
        relPos = new Vector2(0.0f, 0.0f);
        transform.position = To3D(relPos);
        col.isTrigger = true; // 충돌 무시
    }
    void ReleaseBall() {
        state = BallState.free;
        player.GetComponent<PlayerMovement>().state = PlayerMovement.PlayerState.free;
        player = null;
        rb.isKinematic = false;
        transform.parent = null;
        col.isTrigger = false;
        cooltime = 1.0f;
    }
    void HandUpdate() {
        if (player == null) return;
        Transform leftHand = player.transform.Find("LeftHand");
        Transform rightHand = player.transform.Find("RightHand");
        if (state == BallState.dribbled) {
            if (ballDribble.holdingHand == BallDribble.Direction.left) {
                float y = transform.position.y > 10.0f ? transform.position.y + 0.1f : leftHand.position.y;
                leftHand.position = new Vector3 (leftHand.position.x, y, leftHand.position.z);
            }
            if (ballDribble.holdingHand == BallDribble.Direction.right) {
                float y = transform.position.y > 10.0f ? transform.position.y + 0.1f : rightHand.position.y;
                rightHand.position = new Vector3 (rightHand.position.x, y, rightHand.position.z);
            }
        }
         else{
           leftHand.position = new Vector3 (leftHand.position.x, player.transform.position.y, leftHand.position.z);
           rightHand.position = new Vector3 (rightHand.position.x, player.transform.position.y, rightHand.position.z); 
        }
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
            if (ballDribble.isDribbling == false) {
                ReleaseBall();
            }
        }
        else if (state == BallState.recieved || state == BallState.held) {
            transform.position = To3D(relPos);
        }
        HandUpdate();
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
            HoldBall(true);
            ballDribble.isDribbling = false;
        }
    }
    void DribbleLeft() {
        if(state == BallState.recieved || state == BallState.dribbled) {
            ballDribble.isDribbling = true;
            player.GetComponent<PlayerMovement>().state = PlayerMovement.PlayerState.dribbled;
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
            ballDribble.isDribbling = true;
            player.GetComponent<PlayerMovement>().state = PlayerMovement.PlayerState.dribbled;
            rightPressed = true;
            state = BallState.dribbled;
            if (state == BallState.recieved) {
                relPos = new Vector2(1.0f, 0.0f);
                ballDribble.holdingHand = BallDribble.Direction.right;
            }
        }
    }
}

