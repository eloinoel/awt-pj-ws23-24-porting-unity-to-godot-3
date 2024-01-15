using Godot;
using System;
using System.Collections.Generic;

public class DisabilityManager : Node
{
	private List<DisabledNode> disabledNodes = new List<DisabledNode>();

	public override void _Ready()
	{
		base._Ready();
	}

	public void Enable(Node node)
	{
		if (!(node is IDisability)) return;
		if (node == null) return;
		int index = findNodeIndexForNode(node);
		if(index == -1) { return; }

		// reintroduce node to the scene
		NodePath parentPath = disabledNodes[index]._parentPath;
		Node parent = GetNode<Node>(parentPath);
		parent.CallDeferred("add_child", node);

		disabledNodes.RemoveAt(index);

		(node as IDisability).IsActive = true;
		((IDisability) node).OnEnable();
	}

	public void Disable(Node node)
	{
		if (!(node is IDisability)) return;
		if (!(node as IDisability).IsActive) {
			GD.Print((node as IDisability).IsActive);
			GD.PrintErr("DisabilityManager: Disable: Tried disabling inactive node " + node.Name);
			return;
		}

		// get necessary data
		Node parent = node.GetParent();
		NodePath parentPath = parent.GetPath();

		// check if parent has node as child,
		var children = parent.GetChildren();
		if(!children.Contains(node)) {
			GD.PrintErr("DisabilityManager: Disable: Parent " + parent.Name + " doesn't have child " + node.Name);
			return;
		}

		//save node and disable
		(node as IDisability).IsActive = false;
		DisabledNode dNode = new DisabledNode(node, parentPath);
		disabledNodes.Add(dNode);
		parent.CallDeferred("remove_child", node);

		((IDisability) node).OnDisable();
	}

	class DisabledNode
	{
		public NodePath _parentPath;
		public Node _node;

		public DisabledNode(Node node, NodePath parentPath)
		{
			_node = node;
			_parentPath = parentPath;
		}
	}

	private int findNodeIndexForNode(Node node)
	{
		for(int i = 0; i < disabledNodes.Count; i++)
		{
			Node cur = disabledNodes[i]._node;
			if (cur == node)
				return i;
		}
		return -1;
	}
}
