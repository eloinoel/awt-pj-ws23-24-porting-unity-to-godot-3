using Godot;
using System;

public class TimeDisplayItem : Node2D
{
    [Export(hintString: "A reference to the Label to display the time.")]
    protected NodePath displayPath;

    private Label display = null;

    [Export(hintString: "A reference to the Label to display the title for the time.")]
    protected NodePath titlePath;

    private Label title = null;

    public override void _Ready()
    {
        base._Ready();

        try {
            display = GetNode<Label>(displayPath);
            title = GetNode<Label>(titlePath);
        } catch (Exception)
        {
            GD.PrintErr("TimeDisplayItem: Could not get Labels for displaying time or title");
        }
    }

    /// <summary>
    /// Set the text body of the TimeDisplayItem.
    /// If the input "text" is null or empty the method will disable the TimeDisplayItem gameobject. Otherwise it enables it.
    /// </summary>
    /// <param name="text">string to display in the body</param>
    public void SetText(string text)
    {
        if(display == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(text))
        {
            Visible = false;
            return;
        }
        Visible = true;
        display.Text = text;
    }

    /// <summary>
    /// Set the text title of the TimeDisplayItem.
    /// </summary>
    /// <param name="text">string to display in the title</param>
    public void SetTitle(string text)
    {
        if(title == null) { return; }
        title.Text = text;
    }
}
