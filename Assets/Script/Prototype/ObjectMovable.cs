
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
    public class ObjectMovable : NetworkBehaviour
    {
        private sbyte facingDirection;
        private float moveSpeed;
        private ObjectInfoPacked objectInfoPacked;
        private AnimationTransitionState state;
        public ObjectInfoPacked ObjectInfoPacked { get => objectInfoPacked; set => objectInfoPacked = value; }
        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

        private void Awake() 
        {
            objectInfoPacked = GetComponent<ObjectInfoPacked>();    
        }

        private NativeArray<FixedString32Bytes> startWalk = new NativeArray<FixedString32Bytes>
        (
            new FixedString32Bytes[] {new FixedString32Bytes("StartWalk")}, Allocator.Persistent
        );

        private bool[] startWalkBool = new bool[] {true};

        private NativeArray<FixedString32Bytes> startWalk_Walk = new NativeArray<FixedString32Bytes>
        (
            new FixedString32Bytes[] 
            {
                new FixedString32Bytes("StartWalk"),
                new FixedString32Bytes("Walk")
            }, Allocator.Persistent
        );

        private bool[] startWalk_WalkBool = new bool[] {false, false};
        private void Start() 
        {
            
        }

        private void FixedUpdate() 
        {
            Move();
        }

        private void Move()
        {
            if (!IsOwner) return;
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (direction != Vector3.zero)
            {
                facingDirection = (sbyte)(direction.x > 0 ? 1 : -1);
                MoveUsingKeySendToServerRpc(direction, facingDirection);
                if (!objectInfoPacked.Animator.GetBool("StartWalk") && !objectInfoPacked.Animator.GetBool("Walk"))
                {
                    objectInfoPacked.ObjectAnimatorLogic.ChangeBoolServerRpc(startWalk, startWalkBool);
                }
            }
            else if (objectInfoPacked.Animator.GetBool("StartWalk") || objectInfoPacked.Animator.GetBool("Walk"))
            {
                objectInfoPacked.ObjectAnimatorLogic.ChangeBoolServerRpc(startWalk_Walk, startWalk_WalkBool);
            }
        }

        [Rpc(SendTo.Server)]
        public void MoveUsingKeySendToServerRpc(Vector3 direction, sbyte facingDirection)
        {
            transform.localScale = new Vector3(facingDirection, 1, 1);
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }
    }
}