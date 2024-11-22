
using Game;
using Unity.Netcode;
using UnityEngine;

public class CollideAndDamage : NetworkBehaviour
{
	public enum CollideType {Single, Multiple};
	private CollideType collideType = CollideType.Single;
	private float damageDeal = 1f;
	
	/// <summary>
	/// Shouldn't check if it's server in awake because network stuffs is not ready yet
	/// </summary>
	private void Awake() 
	{
		
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		
		if (!IsServer) return;
		
		if (collideType == CollideType.Single)
		{
			onTriggerEnter2DDelegate += (other) => 
			{
				ObjectInfoPacked objectInfoPacked;
				if ((objectInfoPacked = GameManager.Instance.GetObjectInfoPacked(other.gameObject)) != null)
				{
					objectInfoPacked.UpdateHealthServer(-damageDeal);
				}
			};
		}
	}

	
	private delegate void OnTriggerEnter2DDelegate(Collider2D other);
	private OnTriggerEnter2DDelegate onTriggerEnter2DDelegate;
	private void OnTriggerEnter2D(Collider2D other) 
	{
		onTriggerEnter2DDelegate?.Invoke(other);
	}
}