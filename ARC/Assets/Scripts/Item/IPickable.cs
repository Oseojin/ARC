using UnityEngine;

public interface IPickable
{
    void OnPickedUp(Transform holder);
    void OnDropped(Vector3 dropForce);
}
