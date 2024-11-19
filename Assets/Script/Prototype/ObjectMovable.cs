
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
	public class ObjectMovable : NetworkBehaviour
	{
		private sbyte facingDirection;
		private float moveSpeed;
		private NetworkVariable<float> moveSpeedNetVar = new NetworkVariable<float>();
		private ObjectInfoPacked objectInfoPacked;
		private AnimationTransitionState state;
		public ObjectInfoPacked ObjectInfoPacked { get => objectInfoPacked; set => objectInfoPacked = value; }
		public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
		public NetworkVariable<float> MoveSpeedNetVar { get => moveSpeedNetVar; set => moveSpeedNetVar = value; }

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
		
		public override void OnDestroy() 
		{
			startWalk.Dispose();
			startWalk_Walk.Dispose();
		}

		private void FixedUpdate() 
		{
			Move();
		}

		public override void OnNetworkSpawn()
		{
			if (IsOwner)
			{
				GetDataRpc();	
			}
			
			RegisterStateChange();
		}
		
		public void RegisterStateChange()
		{
			moveSpeedNetVar.OnValueChanged += MoveSpeedOnStateChanged;
		}

		public override void OnNetworkDespawn()
		{
			UnRegisterStateChange();
		}

		public void UnRegisterStateChange()
		{
			moveSpeedNetVar.OnValueChanged -= MoveSpeedOnStateChanged;
		}

		private void Move()
		{
			if (!IsOwner) return;
			Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

			if (direction != Vector3.zero)
			{
				facingDirection = (sbyte)(direction.x > 0 ? 1 : -1);
				MoveUsingKeySendToServerRpc(direction, facingDirection);
				StartMoveLogicServerRpc();
			}
			else StopMoveLogicServerRpc();
		}
		
		public void MoveSpeedOnStateChanged(float previous, float current)
		{
			MoveSpeed = current;
		}
		
		[Rpc(SendTo.Server)]
		public void GetDataRpc()
		{
			moveSpeedNetVar.Value = 30f;
		}
		
		[Rpc(SendTo.Server)]
		private void StartMoveLogicServerRpc()
		{
			if (!objectInfoPacked.Animator.GetBool("StartWalk") && !objectInfoPacked.Animator.GetBool("Walk"))
			{
				objectInfoPacked.ObjectAnimatorLogic.ChangeBoolServerRpc(startWalk, startWalkBool);
			}
		}
		
		[Rpc(SendTo.Server)]
		private void StopMoveLogicServerRpc()
		{
			if (objectInfoPacked.Animator.GetBool("StartWalk") || objectInfoPacked.Animator.GetBool("Walk"))
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