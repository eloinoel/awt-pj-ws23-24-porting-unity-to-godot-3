using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class TimeManager : Node, IDisability
{
    public bool IsFinite { get; private set; }
    public float TotalTime { get; private set; }
    public float TimeRemaining { get; private set; }
    public bool IsOver { get; private set; }

    private bool raceStarted;

    public static Action<float> OnAdjustTime;
    public static Action<int, bool, GameMode> OnSetTime;

    private bool isActive = true;
    public bool IsActive
    {
        get => isActive;
        set => isActive = value;
    }

    public override void _Ready()
    {
        base._Ready();
        IsFinite = false;
        TimeRemaining = TotalTime;
    }

    //TODO: maybe needs to be called in _Ready method also
    public void OnEnable()
    {
        OnAdjustTime += AdjustTime;
        OnSetTime += SetTime;
    }

    public void OnDisable()
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

