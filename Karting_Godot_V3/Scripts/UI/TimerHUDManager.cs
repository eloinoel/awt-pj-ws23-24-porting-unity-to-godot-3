using Godot;
using System;

//TODO: delete this File and corresponding node if not needed
//this was probably for the race countdown in Unity
public class TimerHUDManager : Node
{
    //public TextMeshProUGUI timerText;

    [Export]
    NodePath timeManagerPath;
    TimeManager m_TimeManager;

    private void Start()
    {
        m_TimeManager = GetNode<TimeManager>(timeManagerPath);

        /* if (m_TimeManager.IsFinite)
        {
            timerText.text = "";
        } */
    }
    
/*     void Update()
    {
        if (m_TimeManager.IsFinite)
        {   
            timerText.gameObject.SetActive(true);
            int timeRemaining = (int) Math.Ceiling(m_TimeManager.TimeRemaining);
            timerText.text = string.Format("{0}:{1:00}", timeRemaining / 60, timeRemaining % 60);
        }
        else
        {
            timerText.gameObject.SetActive(false);
        }
    } */
}
