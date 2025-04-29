using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterController _cc;
    private Camera _camera;

    [Networked] private float RotationY { get; set; }

    private float _cameraPitch;
    [SerializeField] private float _mouseSensitivity = 2.0f;

    private Vector3 _moveInput;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        _camera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (HasInputAuthority == false)
        {
            // 다른 클라이언트에서는 네트워크 값만 적용
            transform.rotation = Quaternion.Euler(0, RotationY, 0);
            return;
        }

        // 마우스 입력으로 회전값 업데이트
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        RotationY += mouseX;
        RotationY = RotationY % 360f; // 0~360도 정규화

        // 마우스 입력으로 상하 카메라 피치 조정
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;
        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 90f);
        _camera.transform.localEulerAngles = new Vector3(_cameraPitch, 0, 0);

        // 키보드 입력 처리 (WASD)
        _moveInput = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) _moveInput += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) _moveInput += Vector3.back;
        if (Input.GetKey(KeyCode.A)) _moveInput += Vector3.left;
        if (Input.GetKey(KeyCode.D)) _moveInput += Vector3.right;
    }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority == false)
            return;

        // 이동 방향을 현재 회전 방향에 맞춰 변환
        Vector3 forward = Quaternion.Euler(0, RotationY, 0) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0, RotationY, 0) * Vector3.right;
        Vector3 moveDir = forward * _moveInput.z + right * _moveInput.x;

        _cc.Move(5f * moveDir.normalized * Runner.DeltaTime);

        // 항상 transform.rotation 업데이트
        transform.rotation = Quaternion.Euler(0, RotationY, 0);
    }
}
