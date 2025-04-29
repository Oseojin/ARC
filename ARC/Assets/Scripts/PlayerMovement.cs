using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviourPunCallbacks
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    private float originalHeight;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isCrouching = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 내 캐릭터만 카메라 활성화
        if (!photonView.IsMine)
        {
            cameraTransform.gameObject.SetActive(false);
            this.enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        originalHeight = controller.height;
    }

    void Update()
    {
        Move();
        Jump();
        Look();
        HandleCrouch();
    }

    void Jump()
    {
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            if (Input.GetKeyDown(KeyCode.Space) && !isCrouching)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            currentSpeed = runSpeed;
        }
        else if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? crouchHeight : originalHeight;
        }
    }
}