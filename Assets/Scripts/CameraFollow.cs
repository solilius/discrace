using Unity.Netcode;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraFollow : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) gameObject.SetActive(false); 
    }
}