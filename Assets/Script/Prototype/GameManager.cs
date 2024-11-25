using System.Collections.Generic;
using Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : Singleton<GameManager>
{
	public NetworkManager networkManager;
	GameUIManager gameUIManager;
	public bool worldPositionStays = true;
	private void Awake() 
	{
		gameUIManager = GetComponent<GameUIManager>();

		networkManager.ConnectionApprovalCallback += ApprovalCheck;
		networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
		networkManager.OnClientConnectedCallback += OnClientConnectedCallback;

		networkManager.OnServerStarted += () => 
		{
			if (networkManager.IsServer)
			{    
				var parent = Instantiate(Resources.Load<GameObject>("TestParent"));
				var child = Instantiate(Resources.Load<GameObject>("TestChild"));
				var parentNetworkObject = parent.GetComponent<NetworkObject>(); 
				var childNetworkObject = child.GetComponent<NetworkObject>();

				parentNetworkObject.Spawn();
				childNetworkObject.Spawn();
				childNetworkObject.TrySetParent(parentNetworkObject, worldPositionStays);
			}
		};
	}
	
	/// <summary>
	/// Access view variable on start only
	/// </summary>
	private void Start() 
	{
		PopulateView();
		HandleConsole();
		HandleLogin();
	}
	
	TextField usernameField, passwordField, consoleTextField;
	Label messageField, consoleLabel;
	VisualElement authenticationView, adminView;
	ScrollView clientList;
	private void PopulateView()
	{
		usernameField = gameUIManager.AuthenticateView.UsernameField;
		passwordField = gameUIManager.AuthenticateView.PasswordField;
		consoleTextField = gameUIManager.AuthenticateView.ConsoleTextField;
		authenticationView = gameUIManager.AuthenticateView.AuthenticationView;
		adminView = gameUIManager.AuthenticateView.AdminView;
		clientList = gameUIManager.AuthenticateView.ClientList;
		messageField = gameUIManager.AuthenticateView.MessageField;
		consoleLabel = gameUIManager.AuthenticateView.ConsoleLabel;
	}

	// List<string> consoleLines = new List<string>();
	string[] currentConsoleCommand = new string[0];
	string commandOutput = string.Empty;
	public void HandleConsole()
	{
		float speed;
		ulong clientId;
		NetworkClient networkClient;
		ObjectInfoPacked objectInfoPacked;
		consoleTextField.RegisterCallback<KeyDownEvent>
		(
			(evt) => 
			{				
				if (evt.keyCode == KeyCode.Return)
				{
					consoleLabel.text += "\nadmin$ " + $"{consoleTextField.value}";

					currentConsoleCommand = consoleTextField.value.Split(' ');
					switch (currentConsoleCommand[0])
					{
						case "help":
						{
							commandOutput = "add-speed";
							consoleLabel.text += "\n" + commandOutput;
							break;
						}
						case "add-speed":
						{
							if (currentConsoleCommand.Length > 2)
							{
								if (!ulong.TryParse(currentConsoleCommand[1], out clientId))
								{
									commandOutput = "Invalid client id";
									consoleLabel.text += "\n" + commandOutput;
									break;
								}

								if (!float.TryParse(currentConsoleCommand[2], out speed))
								{
									commandOutput = "Invalid speed";
									consoleLabel.text += "\n" + commandOutput;
									break;
								}

								if ((networkClient = networkManager.ConnectedClients[clientId]) == null)
								{
									commandOutput = "Client not found";
									consoleLabel.text += "\n" + commandOutput;
									break;
								}
								else
								{
									objectInfoPacked = networkClient.PlayerObject.GetComponent<ObjectInfoPacked>();
									objectInfoPacked.ObjectMovable.MoveSpeedNetVar.Value += speed;
									commandOutput = "Current speed: " + objectInfoPacked.ObjectMovable.MoveSpeedNetVar.Value;
									consoleLabel.text += "\n" + commandOutput;
								}
							}
							else
							{
								commandOutput = "add-speed {client-id} {speed}";
								consoleLabel.text += "\n" + commandOutput;
							}
							break;
						}
						default:
						{
							commandOutput = "Command not found";
							consoleLabel.text += "\n" + commandOutput;

							break;
						}
					}

					consoleTextField.value = string.Empty;
				}
			}
		, TrickleDown.TrickleDown);
	}

	void AddClientListEntry(string name)
	{
		Label entry = new Label();
		entry.AddToClassList("client-list__entry");
		entry.text = name;
		clientList.Add(entry);
	}

	void OnClientDisconnectCallback(ulong obj)
	{
		if (!networkManager.IsServer && networkManager.DisconnectReason != string.Empty)
		{
			messageField.text = networkManager.DisconnectReason;
			return;
		}
	}

	void OnClientConnectedCallback(ulong obj)
	{
		gameUIManager.DeactivateLayer((int)GameUIManager.LayerUse.AuthenticateView);
		gameUIManager.ActivateLayer((int)GameUIManager.LayerUse.MainView);
	}
	
	private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
	{
		if (string.IsNullOrEmpty(usernameField.value) || string.IsNullOrEmpty(passwordField.value))
		{
			response.Approved = false;
			response.Reason = "Invalid username or password";
			return;
		}
		else
		{
			string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
			string[] credentials = payload.Split(':');
			if (!credentials[0].StartsWith("client"))
			{
				response.Approved = false;
				response.Reason = "Invalid username or password";
				return;
			}

			response.Approved = true;
			response.CreatePlayerObject = true;
			response.PlayerPrefabHash = null;
			response.Position = Vector3.zero;
			response.Rotation = Quaternion.identity;

			AddClientListEntry("#" + request.ClientNetworkId.ToString());
		}
	}
	

	private void HandleLogin()
	{
		gameUIManager.AuthenticateView.onClickLoginButtonDelegate += () => 
		{
			var username = usernameField.value;
			var password = passwordField.value;
			networkManager.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
			
			if (username.Equals("server") && password.Equals("server"))
			{
				// if server exsist, print error message at messageField

				networkManager.StartServer();
				authenticationView.style.display = DisplayStyle.None;
				adminView.style.display = DisplayStyle.Flex;
			} 
			else if (username.Equals("host") && password.Equals("host"))
			{
				// if host exsist, print error message at messageField

				networkManager.StartHost();
				authenticationView.style.display = DisplayStyle.None;
			} 
			else networkManager.StartClient();	
		};
	}

	private void OnClickRegisterButton()
	{

	}
	
	private BinarySearchTree<ObjectInfoPacked> objectInfoPackedTree = new BinarySearchTree<ObjectInfoPacked>();
	public void AddObjectInfoPacked(ObjectInfoPacked objectInfoPacked)
	{
		objectInfoPackedTree.Insert(objectInfoPacked);
	}
	
	public ObjectInfoPacked GetObjectInfoPacked(GameObject gameObject)
	{
		return objectInfoPackedTree.Search((objectInfoPacked) => 
		{
			return objectInfoPacked.gameObject.GetInstanceID();
		}, gameObject.GetInstanceID());
	}
}