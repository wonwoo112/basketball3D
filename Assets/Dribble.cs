using UnityEngine;

public class DribbleController : MonoBehaviour
{
    public Transform ball; // 공의 Transform
    public Transform playerTransform; // 플레이어 Transform
    public float baseDribbleHeight = 1f; // 기본 드리블 높이
    public float dribbleSpeed = 5f; // 드리블 속도
    public float dribbleCooldown = 0.5f; // 드리블 쿨다운 시간

    private float lastDribbleTime; // 마지막 드리블 시간
    private bool goingUp = true; // 공이 위로 이동 중인지 여부

    void Update()
    {
        HandleDribbleMotion();
    }

    // 드리블 모션 처리
    void HandleDribbleMotion()
    {
        // 플레이어 이동 속도를 가져오는 부분 (Rigidbody 사용 가정)
        float playerSpeed = playerTransform.GetComponent<Rigidbody>().velocity.magnitude;

        // 플레이어 속도에 따라 드리블 높이 조정
        float adjustedHeight = Mathf.Clamp(playerSpeed * 0.5f, 0.5f, 1.5f);

        // PingPong 함수를 사용해 부드러운 드리블 구현
        float height = Mathf.PingPong(Time.time * dribbleSpeed, adjustedHeight);

        // 공 위치 업데이트
        ball.position = playerTransform.position + new Vector3(0, baseDribbleHeight + height, 0);

        // 드리블 쿨다운 체크
        if (Time.time - lastDribbleTime > dribbleCooldown)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                Debug.Log("Dribble!");
                lastDribbleTime = Time.time; // 마지막 드리블 시간 갱신
            }
        }
    }
}
