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
}
