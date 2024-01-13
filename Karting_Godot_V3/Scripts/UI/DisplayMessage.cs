using Godot;
using System;

public class DisplayMessage : Node, IDisability
{
    //[TextArea]
    [Export(hintString: "The text that will be displayed")]
    public string message;
    [Export(hintString: "Prefab for the message")]
    //public PoolObjectDef messagePrefab; //TODO: implement pooling if necessary
    public NodePath messagePrefab;
    [Export(hintString: "Delay before displaying the message")]
    public float delayBeforeShowing;

    [Export(hintString: "DisplayMessageManager node path")]
    public NodePath displayMessageManagerPath;


    float m_InitTime = float.NegativeInfinity;

    public bool autoDisplayOnAwake;
    bool m_WasDisplayed;
    DisplayMessageManager m_DisplayMessageManager;

    private NotificationToast notification;


    private bool isActive = true;
    public bool IsActive
    {
        get => isActive;
        set => isActive = value;
    }

    public override void _Ready()
    {
        base._Ready();

        OnEnable(); // In Unity OnEnable is also called after Awake
    }

    public void OnEnable()
    {
        // gets time in micro secs
        m_InitTime = HelperFunctions.GetTime();
        if (m_DisplayMessageManager == null)
            // Prev: m_DisplayMessageManager = FindObjectOfType<DisplayMessageManager>();
            m_DisplayMessageManager = GetNode<DisplayMessageManager>(displayMessageManagerPath);

        // prev: DebugUtility.HandleErrorIfNullFindObject<DisplayMessageManager, DisplayMessage>(m_DisplayMessageManager, this);
        if (m_DisplayMessageManager == null)
            GD.PrintErr("Error at DisplayMessage.cs: DisplayMessageManager is null.");


        m_WasDisplayed = false;
    }

    public void OnDisable() {}

    // Update is called once per frame
    public override void _Process(float delta)
    {
        if (!autoDisplayOnAwake) return;
        if (m_WasDisplayed) return;

        if (HelperFunctions.GetTime() - m_InitTime > delayBeforeShowing) Display();
    }

    public void Display()
    {
        //TODO: port this to godot, when necessary

        /* notification = messagePrefab.getObject(true,m_DisplayMessageManager.DisplayMessageRect.transform).GetComponent<NotificationToast>();

        notification.Initialize(message);

        m_DisplayMessageManager.DisplayMessageRect.UpdateTable(notification.gameObject);

        m_WasDisplayed = true;

        StartCoroutine(messagePrefab.ReturnWithDelay(notification.gameObject,notification.TotalRunTime)); */

    }
}
