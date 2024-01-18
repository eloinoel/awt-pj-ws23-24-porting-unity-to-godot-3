using Godot;
using System;

public class HelperFunctions : Node
{
    /// <summary>
    /// Get current time in seconds, equivalent to Unity's Time.time
    /// </summary>
    public static float GetTime()
    {
        return Time.GetTicksUsec() / 1000000.0f;
    }

    /// <summary>
    /// fade in the node's alpha
    /// you can implement the method OnFadeInComplete, if you want to do something after fading
    /// </summary>
    public void FadeIn(CanvasItem node, float duration = 0.2f)
    {
        var modulate = node.Modulate;
        modulate.a = 0f;
        node.Modulate = modulate;
        node.Visible = true;

        var tween = new Tween();
        node.AddChild(tween);

        tween.InterpolateProperty(node, "modulate:a", 0.0f, 1.0f, duration);

        tween.Start();
    }

    /// <summary>
    /// fade out the node's alpha
    /// you can implement the method OnFadeOutComplete, if you want to do something after fading
    /// </summary>
    public void FadeOut(CanvasItem node, float duration = 0.2f)
    {
        var tween = new Tween();
        node.AddChild(tween);

        tween.InterpolateProperty(node, "modulate:a", 1.0f, 0.0f, duration);

        tween.InterpolateCallback(this, duration, "onFadeOutComplete", node);

        tween.Start();
    }

    private void onFadeOutComplete(CanvasItem node)
    {
        // reset modulate settings
        node.Visible = false;
        var modulate = node.Modulate;
        modulate.a = 1f;
        node.Modulate = modulate;
    }
}
