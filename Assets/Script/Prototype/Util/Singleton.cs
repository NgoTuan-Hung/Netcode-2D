
using Unity.Netcode;
using UnityEngine;

public class Singleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
	private static T instance;
	
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (T)FindFirstObjectByType(typeof(T));
				
				if (instance == null)
				{
					instance = new GameObject(typeof(T).Name).AddComponent<T>();
					instance.gameObject.name = typeof(T).Name;
				}
			}
			
			return instance;
		}
	}
}