using System.Linq;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
	public class ObjectInfoPacked : NetworkBehaviour
	{
		public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
		public GameObject Cinemachine;
		public CinemachineCamera cinemachineCamera;
		private ObjectMovable objectMovable;
		private ObjectAnimatorLogic objectAnimatorLogic;
		private Animator animator;
		public ObjectMovable ObjectMovable { get => objectMovable; set => objectMovable = value; }
		public Animator Animator { get => animator; set => animator = value; }
		public ObjectAnimatorLogic ObjectAnimatorLogic { get => objectAnimatorLogic; set => objectAnimatorLogic = value; }

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

			NetworkManager.Singleton.ConnectedClientsIds.ToList().ForEach(id => Debug.Log(id));
		}
	}
}