using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpPower;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;

    [Header("Wall Climb / Hang")]
    public float wallCheckDistance = 0.6f;
    public float wallClimbSpeed = 2f;
    public LayerMask wallLayerMask;

    private bool isTouchingWall;
    private bool isClimbingWall;
    private bool isHanging;
    public bool isFrozen = false;


    private Vector2 mouseDelta;

    public Action inventory;
    [HideInInspector]
    public bool canLook = true;

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        if (isFrozen) return;

        CheckWall();

        if (isClimbingWall)
        {
            Vector3 verticalDir = Vector3.zero;

            if (curMovementInput.y > 0.1f)
                verticalDir = Vector3.up;
            else if (curMovementInput.y < -0.1f)
                verticalDir = Vector3.down;

            Vector3 horizontalDir = Vector3.zero;

            if (curMovementInput.x != 0)
                horizontalDir = transform.right * curMovementInput.x;

            Vector3 climbDir = (verticalDir + horizontalDir).normalized;
            if (climbDir != Vector3.zero)
                ClimbWall(climbDir);
            else
                ClingWall(); // 입력 없으면 매달리기
        }
        else
        {
            Move();
        }


    }


    private void LateUpdate()
    {
        if (isFrozen || !canLook) return;
        CameraLook();
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    private void Move()
    {
        Vector3 moveDir = new Vector3(curMovementInput.x, 0, curMovementInput.y);
        moveDir = Quaternion.Euler(0, cameraContainer.eulerAngles.y, 0) * moveDir; // 카메라 방향 기준으로 이동
        moveDir *= moveSpeed;
        moveDir.y = rigidbody.velocity.y;

        rigidbody.velocity = moveDir;
    }


    public void OnInventoryButton(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    void CheckWall()
    {
        Ray wallRay = new Ray(transform.position, transform.forward);
        Debug.DrawRay(wallRay.origin, wallRay.direction * wallCheckDistance, Color.red);

        isTouchingWall = Physics.Raycast(wallRay, wallCheckDistance, wallLayerMask);

        if (isTouchingWall)
        {
            isClimbingWall = true;
            rigidbody.useGravity = false;
        }
        else
        {
            isClimbingWall = false;
            rigidbody.useGravity = true;
        }
    }


    void ClimbWall(Vector3 direction)
    {
        rigidbody.velocity = direction * wallClimbSpeed;
    }

    void ClingWall()
    {
        rigidbody.velocity = Vector3.zero;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatForm"))
        {
            transform.SetParent(collision.transform); // 플랫폼에 붙이기
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatForm"))
        {
            transform.SetParent(null); // 떨어질 때 원래 상태로
        }
    }

}