using Godot;
using System;
using System.Collections.Generic;

// TODO: create node for this in the scene
public class TimeDisplay : Node, IDisability
{
    private bool isActive = true;
    public bool IsActive //interface field
    {
        get => isActive;
        set => isActive = value;
    }
    DisabilityManager disabilityManager;

    [Export(hintString: "Display the time for the current lap.")]
    public NodePath currentLapTextPath;
    private TimeDisplayItem currentLapText;
    [Export(hintString: "Display the time for the best lap.")]
    public NodePath bestLaptTextPath;
    private TimeDisplayItem bestLapText;

    /* [Export(hintString: "Pool object for the time display UI item.")]
    public PoolObjectDef timeDisplayItem;  */
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
        //TODO: reenable disabilityManager when inserting in race scene
        disabilityManager = (DisabilityManager) GetTree().GetRoot().GetNode<Node>(GameConstants.disabilityManagerPath);

        currentLapText = GetNode<TimeDisplayItem>(currentLapTextPath);
        bestLapText = GetNode<TimeDisplayItem>(bestLaptTextPath);
        finishedLapsParent = GetNode<UITable>(finishedLapsParentPath);

        currentLapText.SetText("");
        bestLapText.SetText("");
        currentLapText.SetTitle("Current:");
        bestLapText.SetTitle("Best Lap:");
        currentLapStartTime = 0;
        lapsOver = false;

        OnEnable(); // called after awake in Unity
    }

    public void OnEnable()
    {
        OnUpdateLap += UpdateLap;
        OnSetLaps += SetLaps;
    }

    public void OnDisable()
    {
        OnUpdateLap -= UpdateLap;
        OnSetLaps -= SetLaps;
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
        GD.Print("setLaps: laps: " + laps);
        for (int i = 0; i < laps; i++)
        {
            var newItem = (TimeDisplayItem) GD.Load<PackedScene>(TimeDisplayItemPrefabPath).Instance();
            finishedLapsParent.AddChild(newItem);
            //newItem.Position = new Vector2(currentLapText.Position.x, finishedLapsParent.RectPosition.y);
            //disabilityManager.Disable(newItem); //TODO: readd at the end
            //var newItemScaleRect = newItem.GetChild<Control>(0);
            //newItem.Position = new Vector2(newItem.Position.x - newItemScaleRect.RectSize.x / 2, newItem.Position.y - newItemScaleRect.RectSize.y / 2);
            finishedLapsParent.UpdateTable(newItem);

            lapTimesText.Add(newItem);
        }
    }

    TimeDisplayItem GetItem(int i)
    {
        if (i >= lapTimesText.Count)
        {
            TimeDisplayItem newItem = (TimeDisplayItem) GD.Load<PackedScene>(TimeDisplayItemPrefabPath).Instance();
            AddChild(newItem);
            // Prev: newItem.Position = finishedLapsParent.RectPosition;
            newItem.Position = new Vector2(currentLapText.Position.x, finishedLapsParent.RectPosition.y);
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
            // Prev: currentLapText.gameObject.SetActive(false);
            disabilityManager.Disable(currentLapText);
        }
    }


    void AddFinishedLapTime(int lap)
    {
        TimeDisplayItem newItem = GetItem(lap);

        newItem.SetText(getTimeString(finishedLapTimes[lap]));
        newItem.SetTitle($"Lap {lap+1}:");
        // Prev: newItem.gameObject.SetActive(true); // TODO: maybe this should be made differently
        disabilityManager.Enable(newItem);
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
