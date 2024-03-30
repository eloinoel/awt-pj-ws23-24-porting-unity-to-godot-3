using Godot;
using System;

public class TimerHUDManager : Node
{
    [Export]
    NodePath timeManagerPath;
    TimeManager m_TimeManager;

    private void Start()
    {
        m_TimeManager = GetNode<TimeManager>(timeManagerPath);
    }
}
