using System;
using System.Collections;
using System.Xml.Serialization;
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
    }
    
    bool isPossessed() {
        return state != BallState.free;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ball collision");
        if (!isPossessed() && collision.gameObject.CompareTag("Player"))
        {
            // 플레이어의 BallHoldPoint 찾기
            holdPoint = collision.transform.Find("BallHoldPoint");
            Debug.Log("AA");
            if (holdPoint != null)
            {
                Debug.Log("Ball is held");
                HoldBall();
            }
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
    void Update()
    {
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
        state = BallState.free;
    }
    void Pass() {
        state = BallState.free;
    }
    void Hold() {
        state = BallState.held;
    }
    void DribbleLeft() {
        state = BallState.dribbled;
    }
    void DribbleRight() {
        state = BallState.dribbled;
    }
}
