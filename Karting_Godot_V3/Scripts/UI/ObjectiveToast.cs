using Godot;
using System;

public class ObjectiveToast : Node2D
{
	[Export(hintString: "Text content that will display the title")]
	public NodePath titleTextContentPath;
	public Label titleTextContent;
	
	[Export(hintString: "Text content that will display the description")]
	protected NodePath descriptionTextContentPath;
	protected Label descriptionTextContent;
	[Export(hintString: "Text content that will display the counter")]
	public NodePath counterTextContentPath;
	public Label counterTextContent;

	/* [Export(hintString: "Rect that will display the description")]
	public RectTransform subTitleRect;*/
	[Export(hintString: "Canvas used to fade in and out the content")]
	public NodePath canvasGroupPath;
	public Node2D canvasGroup;

	[Export(hintString: "Delay before moving complete")]
	public float completionDelay;
	[Export(hintString: "Duration of the fade in")]
	public float fadeInDuration = 0.5f;
	[Export(hintString: "Duration of the fade out")]
	public float fadeOutDuration = 2f;

	[Export(hintString: "Sound that will be player on initialization")]
	public AudioStream initSound;
	[Export(hintString: "Sound that will be player on completion")]
	public AudioStream completedSound;

	[Export(hintString: "Time it takes to move in the screen")]
	public float moveInDuration = 0.5f;
	/* [Export(hintString: "Animation curve for move in, position in x over time")]
	public AnimationCurve moveInCurve;*/

	[Export(hintString: "Time it takes to move out of the screen")]
	public float moveOutDuration = 2f;
	/*[Export(hintString: "Animation curve for move out, position in x over time")]
	public AnimationCurve moveOutCurve; */

	float m_StartFadeTime;
	bool m_IsFadingIn;
	bool m_IsFadingOut;
	bool m_IsMovingIn;
	bool m_IsMovingOut;
	AudioStreamPlayer m_AudioSource;
	[Export(hintString: "Area that the objectives are displayed in")]
	Transform m_RectTransform; // TODO: Set to ObjectiveToastPrimary node2d in editor

	public void Initialize(string titleText, string descText, string counterText, bool isOptionnal, float delay)
	{
		// set the description for the objective, and forces the content size fitter to be recalculated
		// Canvas.ForceUpdateCanvases(); // TODO: I believe, that because Godot uses Scenes rather than Canvases, that this call is not necessary in godot

		// We rather get this by means of an exported property // TODO: Set exported property m_RectTransform in Editor
/*         m_RectTransform = GetComponent<RectTransform>();
		DebugUtility.HandleErrorIfNullGetComponent<RectTransform, ObjectiveToast>(m_RectTransform, this, gameObject); */
		titleTextContent = GetNode<Label>(titleTextContentPath);
		descriptionTextContent = GetNode<Label>(descriptionTextContentPath);
		counterTextContent = GetNode<Label>(counterTextContentPath);
		canvasGroup = GetNode<Node2D>(canvasGroupPath);

		titleTextContent.Text = titleText;
		SetDescriptionText(descText);
		counterTextContent.Text = counterText;

		m_AudioSource = new AudioStreamPlayer();
		// LayoutRebuilder.ForceRebuildLayoutImmediate(m_RectTransform); // TODO: Here too i imagine that this isnt needed in godot (We'll see ;))

		m_StartFadeTime = HelperFunctions.GetTime() + delay;
		// start the fade in
		m_IsFadingIn = true;
		m_IsMovingIn = true;
	}

	public void Complete()
	{
		m_StartFadeTime = HelperFunctions.GetTime() + completionDelay;
		m_IsFadingIn = false;
		m_IsMovingIn = false;

		// if a sound was set, play it
		PlaySound(completedSound);

		// start the fade out
		m_IsFadingOut = true;
		m_IsMovingOut = true;
	}

	public void SetDescriptionText(string text)
	{
		descriptionTextContent.Text = text;
		// TODO: "see if this is necessary" subTitleRect.gameObject.SetActive(!string.IsNullOrEmpty(text));
	}
	public override void _Process(float delta)
	{
		float timeSinceFadeStarted = HelperFunctions.GetTime() - m_StartFadeTime;

		if (m_IsFadingIn && !m_IsFadingOut)
		{
			// fade in
			if (timeSinceFadeStarted < fadeInDuration)
			{
				// calculate alpha ratio
				// PREV: canvasGroup.alpha = timeSinceFadeStarted / fadeInDuration;
				canvasGroup.Modulate = new Color(1, 1, 1, timeSinceFadeStarted / fadeInDuration);
			}
			else
			{
				// PREV: canvasGroup.alpha = 1f;
				canvasGroup.Modulate = new Color(1, 1, 1, 1);
				// end the fade in
				m_IsFadingIn = false;

				PlaySound(initSound); // TODO: No sound is played after this call ;)
			}
		}

		if (m_IsMovingIn && !m_IsMovingOut)
		{
			float movedOutDistance = 93.5f /* + 18.0f */;
			float hOffset = 18.0f;
			// move in
			if (timeSinceFadeStarted < moveInDuration)
			{
				// PREV: m_RectTransform.anchoredPosition =
				//	new Vector2((int) moveInCurve.Evaluate(timeSinceFadeStarted / moveInDuration),   m_RectTransform.anchoredPosition.y);
				canvasGroup.Position = new Vector2((timeSinceFadeStarted / moveInDuration) * movedOutDistance - movedOutDistance + hOffset, canvasGroup.Position.y);
			}
			else
			{
				// making sure the position is exact
				// PREV: m_RectTransform.anchoredPosition = new Vector2(0,  m_RectTransform.anchoredPosition.y);
				canvasGroup.Position = new Vector2(0.0f + hOffset, canvasGroup.Position.y);

				m_IsMovingIn = false;
			}

		}

        // NOTE: This was not used in the unity Karting project, thus we ignore it ;)
		/*if (m_IsFadingOut)
		{
			// fade out
			if (timeSinceFadeStarted < fadeOutDuration)
			{
				// calculate alpha ratio
				canvasGroup.alpha = 1 - (timeSinceFadeStarted) / fadeOutDuration;
			}
			else
			{
				canvasGroup.alpha = 0f;

				// end the fade out, then destroy the object
				m_IsFadingOut = false;
			   gameObject.SetActive(false);
			}
		}

		if (m_IsMovingOut)
		{
			// move out
			if (timeSinceFadeStarted < moveOutDuration)
			{
				m_RectTransform.anchoredPosition =
					new Vector2((int) moveOutCurve.Evaluate(timeSinceFadeStarted / moveOutDuration),
						m_RectTransform.anchoredPosition.y);

			}
			else
			{
				m_IsMovingOut = false;
			}
		} */
	}

	void PlaySound(AudioStream sound)
	{
		if (sound != null) {
			return;
		}

		if (m_AudioSource != null)
		{
			AddChild(m_AudioSource);
			m_AudioSource.Bus = "HUDObjective";
		}

		// m_AudioSource.PlayOneShot(sound);
		m_AudioSource.Stream = sound;
		m_AudioSource.Play();
	}
}
