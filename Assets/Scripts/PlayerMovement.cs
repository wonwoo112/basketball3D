using System;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    private bool isDashing;
    private float dashTime;
    private enum D {
        u, d, l, r, ul, ur, dl, dr, none
    }
    private D direction;
    private Rigidbody rb;
    

    private Vector3 unitVector(D d) {
        switch (d) {
            case D.u:
                return Vector3.forward;
            case D.d:
                return Vector3.back;
            case D.l:
                return Vector3.left;
            case D.r:
                return Vector3.right;
            case D.ul:
                return (Vector3.forward + Vector3.left) / Mathf.Sqrt(2);
            case D.ur:  
                return (Vector3.forward + Vector3.right) / Mathf.Sqrt(2);
            case D.dl:
                return (Vector3.back + Vector3.left) / Mathf.Sqrt(2);
            case D.dr:
                return (Vector3.back + Vector3.right) / Mathf.Sqrt(2);
            default:
                return Vector3.zero;
        }
    }

    void keyboardInput() {
        int x = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
        int z = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1 : 0);
        if (x == 0 && z == 0) {
            direction = D.none;
        } else if (x == 0) {
            direction = z == 1 ? D.u : D.d;
        } else if (z == 0) {
            direction = x == 1 ? D.r : D.l;
        } else {
            direction = z == 1 ? (x == 1 ? D.ur : D.ul) : (x == 1 ? D.dr : D.dl);
        }
        if (Input.GetKeyDown(KeyCode.Space) && transform.position.y <= 10.1) {
            jump();
        }
    }
    void jump() {
        rb.AddForce(Vector3.up * 15, ForceMode.Impulse);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 10, 0);
        direction = D.none;
        isDashing = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        keyboardInput();
        transform.position = transform.position + ((isDashing ? dashSpeed : baseSpeed) * Time.deltaTime * unitVector(direction));
    }
}
