using Godot;
using System;

public class NotificationToast : Node
{
    [Export(hintString: "Text content that will display the notification text")]
    //prev: public TMPro.TextMeshProUGUI textContent;
    public NodePath labelPath;
    [Export(hintString: "Canvas used to fade in and out the content")]
    public CanvasModulate canvasModulate; // CanvasModulate applies a color tint to all nodes on a canvas. Only one can be used to tint a canvas..
    //prev: public CanvasGroup canvasGroup;
    [Export(hintString: "How long it will stay visible")]
    public float visibleDuration;
    [Export(hintString: "Duration of the fade in")]
    public float fadeInDuration = 0.5f;
    [Export(hintString: "Duration of the fade out")]
    public float fadeOutDuration = 2f;

    float m_InitTime;
    bool m_WasInit;

    public float TotalRunTime => visibleDuration + fadeInDuration + fadeOutDuration;

    public void Initialize(string text)
    {
        Label textContent = GetNode<Label>(labelPath);
        textContent.Text = text;

        m_InitTime = HelperFunctions.GetTime();
        // start the fade out
        m_WasInit = true;
    }

    void Update()
    {
        if (m_WasInit)
        {
            float timeSinceInit = HelperFunctions.GetTime() - m_InitTime;
            if (timeSinceInit < fadeInDuration)
            {
                // fade in
                // prev: canvasGroup.alpha = timeSinceInit / fadeInDuration;
                canvasModulate.Modulate = new Color(1, 1, 1, timeSinceInit/ fadeInDuration);
            }
            else if (timeSinceInit < fadeInDuration + visibleDuration)
            {
                // stay visible
                // prev: canvasGroup.alpha = 1f;
                canvasModulate.Modulate = new Color(1, 1, 1, 1);
            }
            else if (timeSinceInit < fadeInDuration + visibleDuration + fadeOutDuration)
            {
                // fade out
                // prev: canvasGroup.alpha = 1 - (timeSinceInit - fadeInDuration - visibleDuration) / fadeOutDuration;
                canvasModulate.Modulate = new Color(1, 1, 1, 1 - (timeSinceInit - fadeInDuration - visibleDuration) / fadeOutDuration);
            }
            else
            {
                // prev: canvasGroup.alpha = 0f;
                canvasModulate.Modulate = new Color(1, 1, 1, 0);

                // fade out over, destroy the object
                m_WasInit = false;
                //Destroy(gameObject);
            }
        }
    }
}
