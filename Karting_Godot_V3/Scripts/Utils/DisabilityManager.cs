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

        (node as IDisability).IsActive = false;

        //save node
        Node parent = node.GetParent();
        NodePath parentPath = parent.GetPath();
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