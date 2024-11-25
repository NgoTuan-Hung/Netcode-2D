using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GameUIManager : MonoBehaviour
{
	public enum LayerUse
	{
		StartScreen = 0,
		AuthenticateView = 1,
		MainView = 2,
		Config = 3,
		Default = 1
	}
	public static Dictionary<string, VisualElement> helpers = new Dictionary<string, VisualElement>();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void Init()
	{
		helpers = new Dictionary<string, VisualElement>();    
	}

	public static void AddHelper(string name, VisualElement helper)
	{
		helpers.Add(name, helper);
	}

	public static VisualElement GetHelper(string name)
	{
		return helpers[name];
	}

	public static void ChangeAllHelperOpacity(float opacity)
	{
		foreach (var helper in helpers.Values)
		{
			helper.style.opacity = opacity;
		}
	}

	UIDocument mainUIDocument;
	VisualElement root;
	List<VisualElement> layers;

	private MainView mainView;
	private ConfigView configView; 
	private AuthenticateView authenticateView;
	[SerializeField] private VisualTreeAsset configMenuVTA;
	VisualElement configMenu;

	public MainView MainView { get => mainView; set => mainView = value; }
	public ConfigView ConfigView { get => configView; set => configView = value; }
	public AuthenticateView AuthenticateView { get => authenticateView; set => authenticateView = value; }

	private void Awake() 
	{
		EnhancedTouchSupport.Enable();
		mainUIDocument = GetComponent<UIDocument>();
		root = mainUIDocument.rootVisualElement;

		layers = root.Query<VisualElement>(classes: "layer").ToList();
		layers.Sort((ve1, ve2) => ve1.name.CompareTo(ve2.name));
		InitDefaultLayer();

		HandleSafeArea();

		GetViewComponents();
		InstantiateView();
		InitViewComponents();
	}

	private void GetViewComponents()
	{
		mainView = GetComponent<MainView>();
		configView = GetComponent<ConfigView>();
		authenticateView = GetComponent<AuthenticateView>();
		mainView.GameUIManager = configView.GameUIManager = authenticateView.GameUIManager = this;
	}

	private void InstantiateView()
	{
		configMenu = configMenuVTA.Instantiate();
		configMenu.name = "config__menu-root";
		configMenu.style.flexGrow = 1;
		layers[(int)LayerUse.Config].Q(classes:"safe-area").Add(configMenu);
	}

	private void InitViewComponents()
	{
		mainView.Init(mainUIDocument, root);
		configView.Init(mainUIDocument, root);
		authenticateView.Init(mainUIDocument, root);
	}

	public void HandleSafeArea()
	{
		/* Calculate the safe area so UIs don't touch unreachable parts of the screen */
		Rect safeArea = Screen.safeArea;
		Vector2 leftTop = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(safeArea.xMin, Screen.height - safeArea.yMax));
		Vector2 rightBottom = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(Screen.width - safeArea.xMax, safeArea.yMin));
		root.Query<VisualElement>(classes: "safe-area").ForEach((ve) => 
		{
			ve.style.paddingLeft = leftTop.x;
			ve.style.paddingTop = leftTop.y;
			ve.style.paddingRight = rightBottom.x;
			ve.style.paddingBottom = rightBottom.y;
			/*  */
		});
	}

	public void InitDefaultLayer()
	{
		int defaultLayer = (int)LayerUse.Default;
		layers[defaultLayer].style.left = 0;
		layers[defaultLayer].style.top = 0;

		for (int i = 0; i < layers.Count; i++)
		{
			if (i != defaultLayer)
			{
				layers[i].style.left = 99999f;
				layers[i].style.top = 99999f;	
			}
		}
	}

	public void ActivateLayer(int layerIndex)
	{
		layers[layerIndex].style.left = 0;
		layers[layerIndex].style.top = 0;
	}

	public void DeactivateLayer(int layerIndex)
	{
		layers[layerIndex].style.left = 99999f;
		layers[layerIndex].style.top = 99999f;
	}

	public VisualElement GetLayer(int layerIndex)
	{
		return layers[layerIndex];
	}
}