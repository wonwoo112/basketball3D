using System;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    private Rigidbody rb; 
    private Collider col;
    private bool isHeld = false;
    private Transform holdPoint;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ball collision");
        if (!isHeld && collision.gameObject.CompareTag("Player"))
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
        isHeld = true;
        rb.isKinematic = true; // 물리 효과 제거
        rb.velocity = Vector3.zero;
        transform.position = holdPoint.position;
        transform.parent = holdPoint; // 공을 플레이어 손에 고정
        col.isTrigger = true; // 충돌 무시
    }
}
