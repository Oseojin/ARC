using UnityEngine;
using Photon.Pun;

public class ItemBase : MonoBehaviourPun, IPickable
{
    public Rigidbody rb;

    public void OnPickedUp(Transform holder)
    {
        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        rb.isKinematic = true;
        rb.detectCollisions = false;

        photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
    }

    public void OnDropped(Vector3 dropForce)
    {
        transform.SetParent(null);

        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.AddForce(dropForce, ForceMode.Impulse);

        photonView.TransferOwnership(null);
    }
}
