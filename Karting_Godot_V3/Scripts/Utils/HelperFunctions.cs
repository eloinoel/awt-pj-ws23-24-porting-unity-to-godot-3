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
    
    public static float FloatRange(float min = 0.0f, float max = 1.0f) {
        var random = new Random();
        return (float) (random.NextDouble() * (max - min) + min);
    }
}
