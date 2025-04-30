using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviourPun
{
    [Header("이동 설정")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 6f;
    public float jumpForce = 5f;
    public float crouchColY = -0.5f;
    public float crouchHeight = 0.5f;
    public float normalHeight = 2f;

    [Header("마우스 설정")]
    public float mouseSensitivity = 2f;

    [Header("카메라 설정")]
    public Transform cameraHolder; // FPS 카메라가 붙은 객체
    public float crouchCameraY = -0.5f;
    public float normalCameraY = 0.0f;

    [Header("스태미나 설정")]
    public StaminaSystem stamina;

    private Rigidbody rb;
    private CapsuleCollider col;
    private bool isRunning;
    private bool isCrouching;
    private float moveSpeed;

    private float verticalLookRotation = 0f;
    private Transform cam;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        cam = cameraHolder.GetComponentInChildren<Camera>().transform;

        if (!photonView.IsMine)
        {
            cam.gameObject.SetActive(false); // 본인만 카메라 활성화
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        HandleMouseLook();
        HandleMovementInput();
        HandleJump();
        HandleCrouch();
        HandleRun();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        Move();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // 좌우 회전: 캐릭터
        transform.Rotate(Vector3.up * mouseX);

        // 상하 회전: 카메라 (Pitch)
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -85f, 85f);

        cameraHolder.localEulerAngles = new Vector3(verticalLookRotation, 0, 0);
    }

    void HandleMovementInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v).normalized;
        Vector3 moveDir = transform.TransformDirection(inputDir);

        moveSpeed = isRunning && stamina.CanRun() ? runSpeed : walkSpeed;
        Vector3 velocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);
        rb.linearVelocity = velocity;
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            photonView.RPC(nameof(SetCrouchState), RpcTarget.AllBuffered, !isCrouching);
        }
    }

    [PunRPC]
    void SetCrouchState(bool crouch)
    {
        isCrouching = crouch;
        col.height = isCrouching ? crouchHeight : normalHeight;
        col.center = isCrouching ? new Vector3(0, crouchColY, 0) : new Vector3(0, 0, 0);
        cameraHolder.localPosition = new Vector3(0, isCrouching ? crouchCameraY : normalCameraY, 0);
    }

    void HandleRun()
    {
        isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0;
        if (isRunning)
        {
            stamina.UseStamina(Time.deltaTime * 20f);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, col.bounds.extents.y + 0.1f);
    }

    void Move() { /* Rigidbody.velocity는 HandleMovementInput에서 처리 */ }
}
