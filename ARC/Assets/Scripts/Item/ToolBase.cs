using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public abstract class ToolBase : MonoBehaviourPun, IUsable
{
    public abstract void Use(); // ��ӹ޾� ���� ��� ����

    protected void LogUse()
    {
        Debug.Log($"{gameObject.name} ���� ����");
    }
}
