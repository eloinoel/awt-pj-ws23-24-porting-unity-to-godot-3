using Godot;
using System;

public class UITable : Node
{
    [Export(hintString:"How much space should there be between items?")]
    public float offset = 25;

    [Export(hintString:"Add new the new items below existing items.")]
    public bool down = true;

    public void UpdateTable(Node newItem)
    {
        //if (newItem != null) newItem.GetNode<Control>("ScaleRect").RectScale = new Vector2(1f, 1f); //TODO: custom scaling probably makes this obsolete?

        int childCount = GetChildCount();

        //RectTransform hi = GetComponent<RectTransform>(); //this is the RecTransform

        float height = 0;
        for (int i = 0; i < childCount; i++)
        {
            Control childRect = GetChild<Node>(i).GetNode<Control>("ScaleRect");
            Vector2 size = childRect.RectSize;
            height += down ? -size.y : size.y;
            if (i != 0) height += down? -offset : offset;

            Vector2 newPos = new Vector2();

            newPos.y = height;
            newPos.x = 0;//-child.pivot.x * size.x * hi.localScale.x;
            childRect.RectPosition = newPos;
        }
    }
}
