using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class PlayerNetworkSync : MonoBehaviourPun
{
    [Header("컴포넌트")]
    public Animator animator;
    public Rigidbody rb;

    private Vector3 lastPosition;
    private float moveThreshold = 0.05f;

    void Start()
    {
        if (!photonView.IsMine)
        {
            rb.isKinematic = true; // ���� �÷��̾�� ���� ���� ����
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        AnimateMovement();
    }

    void AnimateMovement()
    {
        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontal = new Vector3(velocity.x, 0, velocity.z);

        float speed = horizontal.magnitude;
        bool isMoving = speed > moveThreshold;

        animator.SetBool("isMoving", isMoving);
        animator.SetFloat("speed", speed);
    }
}
