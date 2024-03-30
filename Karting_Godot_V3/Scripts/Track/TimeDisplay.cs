using Godot;
using System;
using System.Collections.Generic;

public class TimeDisplay : Node
{
    [Export(hintString: "Display the time for the current lap.")]
    public NodePath currentLapTextPath;
    private TimeDisplayItem currentLapText;
    [Export(hintString: "Display the time for the best lap.")]
    public NodePath bestLaptTextPath;
    private TimeDisplayItem bestLapText;

    [Export(hintString:"Prefab to TimeDisplayItem")]
    public string TimeDisplayItemPrefabPath = "res://Scenes/GameHUD/LapTimeCanvas/TimeDisplayItem.tscn";

    [Export(hintString: "Finished lap info will be displayed under this parent.")]
    public NodePath finishedLapsParentPath;
    private UITable finishedLapsParent;

    public static Action OnUpdateLap;
    public static Action<int> OnSetLaps;

    private List<float> finishedLapTimes = new List<float>();

    private float currentLapStartTime;

    private List<TimeDisplayItem> lapTimesText = new List<TimeDisplayItem>();

    private bool lapsOver;

    public override void _Ready()
    {
        base._Ready();

        currentLapText = GetNode<TimeDisplayItem>(currentLapTextPath);
        bestLapText = GetNode<TimeDisplayItem>(bestLaptTextPath);
        finishedLapsParent = GetNode<UITable>(finishedLapsParentPath);

        currentLapText.SetText("");
        bestLapText.SetText("");
        currentLapText.SetTitle("Current:");
        bestLapText.SetTitle("Best Lap:");
        currentLapText.Visible = false;
        bestLapText.Visible = false;
        currentLapStartTime = 0;
        lapsOver = false;

        OnEnable(); // called after awake in Unity
    }

    //this callback will not work in Godot with disabilityManager Plugin we wrote for this project
    public void OnEnable()
    {
        OnUpdateLap += UpdateLap;
        OnSetLaps += SetLaps;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (currentLapStartTime == 0) return;
        if (lapsOver) return;

        currentLapText.SetText(DisplayCurrentLapTime());
    }

    void SetLaps(int laps)
    {
        for (int i = 0; i < laps; i++)
        {
            var newItem = (TimeDisplayItem) GD.Load<PackedScene>(TimeDisplayItemPrefabPath).Instance();
            finishedLapsParent.AddChild(newItem);
            newItem.Visible = false;
            finishedLapsParent.UpdateTable(newItem);

            lapTimesText.Add(newItem);
        }
    }

    TimeDisplayItem GetItem(int i)
    {
        if (i >= lapTimesText.Count)
        {
            TimeDisplayItem newItem = (TimeDisplayItem) GD.Load<PackedScene>(TimeDisplayItemPrefabPath).Instance();
            finishedLapsParent.AddChild(newItem);
            finishedLapsParent.UpdateTable(newItem);
            lapTimesText.Add(newItem);
            return newItem;
        }
        return lapTimesText[i];
    }

    int getBestLap()
    {
        int best = -1;
        for (int i = 0; i < finishedLapTimes.Count; i++)
        {
            if (best < 0 || finishedLapTimes[i] < finishedLapTimes[best]) best = i;
        }

        return best;
    }

    void UpdateLap()
    {
        if (lapsOver) return;

        if (currentLapStartTime == 0)
        {
            currentLapStartTime = HelperFunctions.GetTime();
            return;
        }

        finishedLapTimes.Add(HelperFunctions.GetTime() - currentLapStartTime);
        currentLapStartTime = HelperFunctions.GetTime();

        AddFinishedLapTime(finishedLapTimes.Count - 1);

        bestLapText.SetText(DisplaySessionBestLapTime());

        if (finishedLapTimes.Count == lapTimesText.Count)
        {
            lapsOver = true;
            currentLapText.Visible = false;
        }
    }


    void AddFinishedLapTime(int lap)
    {
        TimeDisplayItem newItem = GetItem(lap);

        newItem.SetText(getTimeString(finishedLapTimes[lap]));
        newItem.SetTitle($"Lap {lap+1}:");
        newItem.Visible = true;
    }

    string DisplayCurrentLapTime()
    {
        float currentLapTime = HelperFunctions.GetTime() - currentLapStartTime;
        if (currentLapTime < 0.01f) return "0:00";
        return getTimeString(currentLapTime);
    }

    string getTimeString(float time){
        int timeInt = (int)(time);
        int minutes = timeInt / 60;
        int seconds = timeInt % 60;
        float fraction = (time * 100) % 100;
        if (fraction > 99) fraction = 99;
        return string.Format("{0}:{1:00}:{2:00}", minutes, seconds, fraction);
    }

    string DisplaySessionBestLapTime()
    {
        int bestLap = getBestLap();
        if (bestLap < 0) return "";
        return getTimeString(finishedLapTimes[bestLap]);
    }
}
