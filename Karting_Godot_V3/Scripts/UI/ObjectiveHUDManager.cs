using System;
using Godot;
using Dictionary = System.Collections.Generic.Dictionary<Objective, ObjectiveToast>;

public class ObjectiveHUDManager : Node
{
/*     [Export(hintString: "UI panel containing the layoutGroup for displaying objectives")]
	public UITable objectivePanel;
	[Export(hintString: "Prefab for the primary objectives")]
	public PoolObjectDef primaryObjectivePrefab;
	[Export(hintString: "Prefab for the primary objectives")]
	public PoolObjectDef secondaryObjectivePrefab;*/
	[Export(hintString: "Prefab for the primary objectives")]
	public NodePath primaryObjectivePrefabPath;
	public ObjectiveToast primaryObjectivePrefab;

	System.Collections.Generic.Dictionary<Objective, ObjectiveToast> m_ObjectivesDictionary;

	public override void _Ready()
	{
		m_ObjectivesDictionary = new System.Collections.Generic.Dictionary<Objective, ObjectiveToast>();
/* 		Node node = GetNode<Node>(primaryObjectivePrefabPath);
		primaryObjectivePrefab = GetNode<ObjectiveToast>(node.GetChild(0).GetPath()); */
		primaryObjectivePrefab = GetNode<ObjectiveToast>(primaryObjectivePrefabPath);
	}

	public void RegisterObjective(Objective objective)
	{
		// PREV: objective.onUpdateObjective += OnUpdateObjective;
		objective.onUpdateObjective += OnUpdateObjective;

		// instanciate the Ui element for the new objective
		/* GameObject objectiveUIInstance = objective.isOptional ? secondaryObjectivePrefab.getObject(true, objectivePanel.transform) : primaryObjectivePrefab.getObject(true, objectivePanel.transform);

		if (!objective.isOptional)
			objectiveUIInstance.transform.SetSiblingIndex(0);

		ObjectiveToast toast = objectiveUIInstance.GetComponent<ObjectiveToast>();
		DebugUtility.HandleErrorIfNullGetComponent<ObjectiveToast, ObjectiveHUDManger>(toast, this, objectiveUIInstance.gameObject);

		// initialize the element and give it the objective description
		toast.Initialize(objective.title, objective.description, objective.GetUpdatedCounterAmount(), objective.isOptional, objective.delayVisible);

		m_ObjectivesDictionary.Add(objective, toast);

		objectivePanel.UpdateTable(toast.gameObject);*/
		ObjectiveToast toast = primaryObjectivePrefab;
		toast.Initialize(objective.title, objective.description, objective.GetUpdatedCounterAmount(), objective.isOptional, objective.delayVisible);

		m_ObjectivesDictionary.Add(objective, toast);

		//objectivePanel.UpdateTable(toast.gameObject);
		//((CanvasItem) toast).Visible = true;
		//toast.Modulate = new Color(1, 1, 1, 0);
	}

	public void UnregisterObjective(Objective objective)
	{
		/* TODO: objective.onUpdateObjective -= OnUpdateObjective;

		// if the objective if in the list, make it fade out, and remove it from the list
		if (m_ObjectivesDictionary.TryGetValue(objective, out ObjectiveToast toast))
			toast.Complete();
		
		m_ObjectivesDictionary.Remove(objective); */
	}

	void OnUpdateObjective(ActionUpdateObjective updateObjective)
	{
		if (m_ObjectivesDictionary.TryGetValue(updateObjective.objective, out ObjectiveToast toast))
		{
			// set the new updated description for the objective, and forces the content size fitter to be recalculated
			// Canvas.ForceUpdateCanvases();
			if (!string.IsNullOrEmpty(updateObjective.descriptionText))
				toast.SetDescriptionText(updateObjective.descriptionText);

 			if (!string.IsNullOrEmpty(updateObjective.counterText))
				toast.SetCounterText(updateObjective.counterText);
			
			GD.Print("Update Lap Counter");

			/*RectTransform toastRectTransform = toast.GetComponent<RectTransform>();
			if (toastRectTransform != null) UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(toastRectTransform); */
		}
	}
}
