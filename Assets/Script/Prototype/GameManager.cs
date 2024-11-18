using Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : NetworkBehaviour
{
    UIDocument uIDocument;
    VisualElement rootVisualElement, loginButton, registerButton, authenticationView, adminView;
    ScrollView adminPanel, clientList;
    TextField usernameField, passwordField, consoleTextField;
    Label messageField, consoleLabel;
    public NetworkManager networkManager;
    public bool worldPositionStays = true;
    private void Awake() 
    {
        HandleUI();

        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
        networkManager.OnClientConnectedCallback += OnClientConnectedCallback;

        networkManager.OnServerStarted += () => 
        {
            if (networkManager.IsServer)
            {
                print("Server started");
                
                var parent = Instantiate(Resources.Load<GameObject>("TestParent"));
                var child = Instantiate(Resources.Load<GameObject>("TestChild"));
                var parentNetworkObject = parent.GetComponent<NetworkObject>(); 
                var childNetworkObject = child.GetComponent<NetworkObject>();

                parentNetworkObject.Spawn();
                childNetworkObject.Spawn();
                childNetworkObject.TrySetParent(parentNetworkObject, worldPositionStays);

                print(networkManager.SpawnManager.SpawnedObjects.Count);
            }
        };
    }

    void HandleUI()
    {
        uIDocument = GetComponent<UIDocument>();    
        rootVisualElement = uIDocument.rootVisualElement;
        authenticationView = rootVisualElement.Q<VisualElement>(name: "authentication-view");
        adminView = rootVisualElement.Q<VisualElement>(name: "admin-view");
        adminPanel = rootVisualElement.Q<ScrollView>(name: "admin-view__admin-panel");
        clientList = adminPanel.Q<ScrollView>(name: "container__client-list");
        consoleTextField = adminPanel.Q<TextField>(name: "console-container__text-field");
        consoleLabel = adminPanel.Q<Label>(name: "admin-console-scroll-view__label");
        loginButton = authenticationView.Q<VisualElement>(name: "authentication-view__login-button");
        registerButton = authenticationView.Q<VisualElement>(name: "authentication-view__register-button");
        usernameField = authenticationView.Q<TextField>(name: "authentication-view__username-textfield");
        passwordField = authenticationView.Q<TextField>(name: "authentication-view__password-textfield");
        messageField = authenticationView.Q<Label>(name: "authentication-view__message");
        loginButton.RegisterCallback<MouseDownEvent>((evt) => OnClickLoginButton());
        registerButton.RegisterCallback<PointerDownEvent>((evt) => OnClickRegisterButton());

        HandleConsole();
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
                                    objectInfoPacked.MoveSpeed.Value += speed;
                                    commandOutput = "Current speed: " + objectInfoPacked.MoveSpeed.Value;
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
        authenticationView.style.display = DisplayStyle.None;
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

    private void OnClickLoginButton()
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
    }

    private void OnClickRegisterButton()
    {

    }
}