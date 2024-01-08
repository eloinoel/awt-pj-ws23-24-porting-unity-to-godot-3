using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class TimeManager : Node
{
    public bool IsFinite { get; private set; }
    public float TotalTime { get; private set; }
    public float TimeRemaining { get; private set; }
    public bool IsOver { get; private set; }

    private bool raceStarted;

    public static Action<float> OnAdjustTime;
    public static Action<int, bool, GameMode> OnSetTime;

    public override void _Ready()
    {
        base._Ready();
        IsFinite = false;
        TimeRemaining = TotalTime;
    }

    //TODO: this callback doesn't work in GODOT, maybe use public override void _EnterTree()
    void OnEnable()
    {
        OnAdjustTime += AdjustTime;
        OnSetTime += SetTime;
    }

    //TODO: this callback doesn't work in GODOT, maybe use public override void _ExitTree()
    private void OnDisable()
    {
        OnAdjustTime -= AdjustTime;
        OnSetTime -= SetTime;
    }

    private void AdjustTime(float delta)
    {
        TimeRemaining += delta;
    }

    private void SetTime(int time, bool isFinite, GameMode gameMode)
    {
        TotalTime = time;
        IsFinite = isFinite;
        TimeRemaining = TotalTime;
    }

    public override void _Process(float delta) // delta is in seconds
    {
        base._Process(delta);

        if (!raceStarted) return;

        if (IsFinite && !IsOver)
        {
            TimeRemaining -= delta;
            if (TimeRemaining <= 0)
            {
                TimeRemaining = 0;
                IsOver = true;
            }
        }
    }

    public void StartRace()
    {
        raceStarted = true;
    }

    public void StopRace() {
        raceStarted = false;
    }
}

