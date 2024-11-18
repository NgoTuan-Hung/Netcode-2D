using System;
using Unity.Netcode;
using UnityEngine;

public class GroundCheck : NetworkBehaviour
{
    private readonly int layerMask = Physics2D.AllLayers;
    private bool isGround = false;
    private NetworkVariable<bool> isGroundNetworkVariable = new NetworkVariable<bool>(false);
    private float rayCastDistance = Mathf.Infinity;
    public int LayerMask => layerMask;
    public bool IsGround { get => isGround; set => isGround = value; }
    public float RayCastDistance { get => rayCastDistance; set => rayCastDistance = value; }
    public RaycastHit2D Hit { get => hit; set => hit = value; }

    private void Awake() 
    {
        isGroundNetworkVariable.OnValueChanged += IsGroundOnValueChanged;    
    }

    void IsGroundOnValueChanged(bool previous, bool current)
    {
        isGround = current;
    }

    void FixedUpdate()
    {
        if (IsLocalPlayer) ServerRayCastCheckRpc();
    }

    private RaycastHit2D hit;
    [Rpc(SendTo.Server)]
    void ServerRayCastCheckRpc()
    {
        if (hit = Physics2D.Raycast(transform.position, Vector2.down, rayCastDistance, layerMask))
        {
            isGroundNetworkVariable.Value = true;
        }
        else
        {
            isGroundNetworkVariable.Value = false;
        }
    }
}