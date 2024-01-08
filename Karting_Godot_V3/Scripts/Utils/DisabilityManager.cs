using Godot;
using System;
using System.Collections.Generic;

public class DisabilityManager : Node
{
    private List<DisabledNode> disabledNodes;

    public override void _Ready()
    {
        base._Ready();
        GD.Print(this.GetPath());
    }

    public void Enable(Node node)
    {
        if (!(node is IDisability)) return;
        if (node == null)
        {
            GD.PrintErr("Enable: Node is null");
            return;
        }
        GD.PrintErr(node);

        // reintroduce node to the scene
        int index = findNodeIndexForNode(node);
        if(index == -1) { GD.PrintErr("DisabilityManger: Could not find node: " + node); }
        NodePath parentPath = disabledNodes[index]._parentPath;
        Node parent = GetNode<Node>(parentPath);
        parent.AddChild(node);

        disabledNodes.RemoveAt(index);

        (node as IDisability).isActive = true;
        ((IDisability) node).OnEnable();
    }

    public void Disable(Node node)
    {
        if (!(node is IDisability)) return;

        //save node
        Node parent = node.GetParent();
        NodePath parentPath = parent.GetPath();
        DisabledNode dNode = new DisabledNode(node, parentPath);
        disabledNodes.Add(dNode);

        parent.RemoveChild(node);

        (node as IDisability).isActive = false;
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