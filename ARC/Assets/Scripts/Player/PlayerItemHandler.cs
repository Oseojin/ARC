using UnityEngine;
using Photon.Pun;

public class PlayerItemHandler : MonoBehaviourPun
{
    public Camera playerCamera;
    public Transform itemHoldPoint;
    private GameObject heldItem;

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            if (heldItem != null)
            {
                photonView.RPC(nameof(RPC_DropItem), RpcTarget.AllBuffered);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryUseTool();
        }
    }

    void TryInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            if (hit.collider.TryGetComponent(out ItemBase item))
            {
                if (heldItem != null)
                {
                    photonView.RPC(nameof(RPC_DropItem), RpcTarget.AllBuffered);
                }

                photonView.RPC(nameof(RPC_PickUpItem), RpcTarget.AllBuffered, item.photonView.ViewID);
            }
        }
    }

    void TryUseTool()
    {
        if (heldItem == null) return;

        photonView.RPC(nameof(RPC_UseTool), RpcTarget.AllBuffered, heldItem.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void RPC_UseTool(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view == null) return;

        var usable = view.GetComponent<IUsable>();
        usable?.Use();
    }

    [PunRPC]
    void RPC_PickUpItem(int viewID)
    {
        PhotonView itemView = PhotonView.Find(viewID);
        if (itemView == null) return;

        GameObject itemObj = itemView.gameObject;
        heldItem = itemObj;

        itemObj.GetComponent<IPickable>()?.OnPickedUp(itemHoldPoint);
    }


    [PunRPC]
    void RPC_DropItem()
    {
        if (heldItem == null) return;

        Vector3 forceDir = transform.forward * 3f;
        heldItem.GetComponent<IPickable>()?.OnDropped(forceDir);
        heldItem = null;
    }

}
