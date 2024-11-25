
using System.Security;
using UnityEngine.UIElements;

public class AuthenticateView : ViewBase
{
	VisualElement loginButton, registerButton, authenticationView, adminView;
	ScrollView adminPanel, clientList;
	TextField usernameField, passwordField, consoleTextField;
	Label messageField, consoleLabel;
	public override void Init(UIDocument uIDocument, VisualElement rootVisualElement)
	{
		base.Init(uIDocument, rootVisualElement);
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
	}

	public delegate void OnClickLoginButtonDelegate();
	public OnClickLoginButtonDelegate onClickLoginButtonDelegate;
	private void OnClickLoginButton()
	{
		onClickLoginButtonDelegate?.Invoke();
	}

	public delegate void OnClickRegisterButtonDelegate();
	public OnClickRegisterButtonDelegate onClickRegisterButtonDelegate;

    public VisualElement LoginButton { get => loginButton; set => loginButton = value; }
    public VisualElement RegisterButton { get => registerButton; set => registerButton = value; }
    public VisualElement AuthenticationView { get => authenticationView; set => authenticationView = value; }
    public VisualElement AdminView { get => adminView; set => adminView = value; }
    public ScrollView AdminPanel { get => adminPanel; set => adminPanel = value; }
    public ScrollView ClientList { get => clientList; set => clientList = value; }
    public TextField UsernameField { get => usernameField; set => usernameField = value; }
    public TextField PasswordField { get => passwordField; set => passwordField = value; }
    public TextField ConsoleTextField { get => consoleTextField; set => consoleTextField = value; }
    public Label MessageField { get => messageField; set => messageField = value; }
    public Label ConsoleLabel { get => consoleLabel; set => consoleLabel = value; }

    private void OnClickRegisterButton()
	{
		onClickRegisterButtonDelegate?.Invoke();
	}
}