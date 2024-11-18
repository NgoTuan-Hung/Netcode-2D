
using Unity.Netcode;
using UnityEngine;

public class TestNetworkObjectSpawned : NetworkBehaviour
{
    protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
    {
        print("OnSynchronize");
        base.OnSynchronize(ref serializer);
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawned");
    }
}