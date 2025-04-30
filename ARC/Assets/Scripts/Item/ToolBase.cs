using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public abstract class ToolBase : MonoBehaviourPun, IUsable
{
    public abstract void Use(); // 상속받아 실제 기능 구현

    protected void LogUse()
    {
        Debug.Log($"{gameObject.name} 도구 사용됨");
    }
}
