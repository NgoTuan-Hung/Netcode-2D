using System;
using System.Linq;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
	public class ObjectInfoPacked : NetworkBehaviour, IComparable<ObjectInfoPacked>
	{
		public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
		private NetworkVariable<float> health = new NetworkVariable<float>(100);
		public GameObject Cinemachine;
		public CinemachineCamera cinemachineCamera;
		private ObjectMovable objectMovable;
		private ObjectAnimatorLogic objectAnimatorLogic;
		private Animator animator;
		public ObjectMovable ObjectMovable { get => objectMovable; set => objectMovable = value; }
		public Animator Animator { get => animator; set => animator = value; }
		public ObjectAnimatorLogic ObjectAnimatorLogic { get => objectAnimatorLogic; set => objectAnimatorLogic = value; }
		public NetworkVariable<float> Health { get => health; set => health = value; }

		private void Awake() 
		{
			PopulateComponents();
		}

		public void PopulateComponents()
		{
			objectMovable = GetComponent<ObjectMovable>();
			objectAnimatorLogic = GetComponent<ObjectAnimatorLogic>();
			animator = GetComponent<Animator>();
		}

		public override void OnNetworkSpawn()
		{
			if (IsOwner)
			{
				Cinemachine = GameObject.Find("CinemachineCamera");
				cinemachineCamera = Cinemachine.GetComponent<CinemachineCamera>();
				cinemachineCamera.Target.TrackingTarget = transform;
			}
			
			if (IsServer)
			{
				GameManager.Instance.AddObjectInfoPacked(this);
			}

			NetworkManager.Singleton.ConnectedClientsIds.ToList().ForEach(id => Debug.Log(id));
		}

		public int CompareTo(ObjectInfoPacked other)
		{
			return gameObject.GetInstanceID().CompareTo(other.gameObject.GetInstanceID());
		}
		
		public void UpdateHealthServer(float health)
		{
			this.health.Value += health;
			print(this.health.Value);
		}
	}
}