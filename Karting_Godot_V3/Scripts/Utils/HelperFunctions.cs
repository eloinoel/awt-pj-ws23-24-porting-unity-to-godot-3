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
    public static void FadeIn(CanvasItem node, float duration)
    {
        var modulate = node.Modulate;
        modulate.a = 0f;
        node.Modulate = modulate;
        node.Visible = true;

        var tween = new Tween();
        node.AddChild(tween);

        tween.InterpolateProperty(node, "modulate:a", 0.0f, 1.0f, duration);

        tween.Connect("fade_in_complete", node, "OnFadeInComplete");

        tween.Start();
    }

    /// <summary>
    /// fade out the node's alpha
    /// you can implement the method OnFadeOutComplete, if you want to do something after fading
    /// </summary>
    public static void FadeOut(CanvasItem node, float duration)
    {
        var tween = new Tween();
        node.AddChild(tween);

        tween.InterpolateProperty(node, "modulate:a", 1.0f, 0.0f, duration);

        tween.Connect("fade_in_complete", node, "OnFadeOutComplete");

        tween.Start();
    }
}
