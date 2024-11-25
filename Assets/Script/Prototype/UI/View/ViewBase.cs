using UnityEngine;
using UnityEngine.UIElements;

public class ViewBase : MonoBehaviour
{
	protected UIDocument uIDocument;
	protected VisualElement rootVisualElement, layerBase, viewBaseVisualElement;
	protected GameUIManager gameUIManager;
	public GameUIManager GameUIManager { get => gameUIManager; set => gameUIManager = value; }
	public UIDocument UIDocument { get => uIDocument; set => uIDocument = value; }
	public VisualElement RootVisualElement { get => rootVisualElement; set => rootVisualElement = value;}
	public VisualElement LayerBase { get => layerBase; set => layerBase = value; }
	public VisualElement ViewBaseVisualElement { get => viewBaseVisualElement; set => viewBaseVisualElement = value; }
	
	public virtual void Init(UIDocument uIDocument, VisualElement rootVisualElement)
	{
		this.uIDocument = uIDocument;
		this.rootVisualElement = rootVisualElement;
	}
}