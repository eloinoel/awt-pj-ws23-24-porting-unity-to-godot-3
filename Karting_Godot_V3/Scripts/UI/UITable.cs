using Godot;
using System;

public class UITable : Control
{
    [Export(hintString:"How much space should there be between items?")]
    public float offset = 7;

    [Export(hintString:"Add new the new items below existing items.")]
    public bool down = true;

    public void UpdateTable(Node newItem)
    {
        float scaling = 0.8f;
        float xdiff = 0f; // used to determine x position offset later on
        if (newItem != null)
        {
            //make finished lap times rects smaller
            var scaleRect = newItem.GetNode<Control>("ScaleRect");
            xdiff = scaleRect.RectSize.x * scaleRect.RectScale.x;
            scaleRect.RectScale = new Vector2(scaleRect.RectScale.x * scaling, scaleRect.RectScale.y * scaling);
            xdiff -= scaleRect.RectSize.x * scaleRect.RectScale.x;
        }

        int childCount = GetChildCount();

        float height = 0;
        for (int i = 0; i < childCount; i++)
        {
            Node2D child = GetChild<Node2D>(i);
            Control childScaleRect = GetChild<Node>(i).GetNode<Control>("ScaleRect");
            Vector2 size = childScaleRect.RectSize;

            if (i != 0)
            {
                height += down ? size.y * childScaleRect.RectScale.y : -size.y * childScaleRect.RectScale.y;
                height += down? offset : -offset;
            }

            Vector2 newPos = new Vector2(xdiff / 2, height);
            child.Position = newPos;
        }
    }
}
